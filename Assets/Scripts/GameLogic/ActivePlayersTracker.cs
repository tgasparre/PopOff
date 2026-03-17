using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public interface IActivePlayerTracker
{
	public void SpawnPlayers();
	public void DestroyPlayers();
	public void SetPlayerStates(PlayerState state);
	public PlayerController[] GetPlayers();
	public void SubscribeMiniGameDeath(Action<Player> onPlayerDiedInMinigame);
}
public class ActivePlayersTracker : MonoBehaviour, IActivePlayerTracker
{
	public const int MAX_PLAYER = 4;
	
	private PlayerInputManager _inputManager;
	private Transform _playerJail;
	
	private Coroutine _joinCoroutine;
	private bool _canJoin = false;
	public bool CanJoin
	{
		get => _canJoin;
		set
		{
			_canJoin = value;
			if (value) WaitForPlayerJoined();
		}
	}

	private struct PlayerTrack
	{
		public PlayerController controller;
		public bool isDead;
		public int PlayerIndex => controller.PlayerIndex;

		public PlayerTrack(PlayerController c)
		{
			controller = c;
			isDead = false;
		}
	}

	private PlayerTrack[] _players = new PlayerTrack[MAX_PLAYER];
	private PlayerTrack[] _activePlayers = Array.Empty<PlayerTrack>();
	private PlayerTrack[] _alivePlayers => _activePlayers.Where(t => !t.isDead).ToArray();
	public PlayerController[] GetPlayers()
	{
		return _activePlayers.Select(tracker => tracker.controller).ToArray(); 
	}
	public int WinningPlayerIndex { get; private set; } = -1;
	public int joinedPlayerCount { get; private set; } = 0;
	
	private bool isFrozen = false;
	public bool IsPlayersFrozen
	{
		get => isFrozen;
		set
		{
			foreach (PlayerTrack tracker in _activePlayers)
			{
				tracker.controller.SetInputEnabled(!value);
			}
			isFrozen = value;
		}
	}

	public static event Action JoinEnded;
	public static event Action<PlayerController> Joined;
	
	private Action<Player> _onPlayerDiedInMinigame;
	public void SubscribeMiniGameDeath(Action<Player> onPlayerDiedInMinigame)
	{
		_onPlayerDiedInMinigame = onPlayerDiedInMinigame;
	}

	private void Awake()
	{
		_inputManager = GetComponent<PlayerInputManager>();
		_inputManager.onPlayerJoined += OnPlayerJoined;
		_playerJail = transform.GetChild(0);
	}

	private void OnDestroy()
	{
		_inputManager.onPlayerJoined -= OnPlayerJoined;
	}

	#region Player Joining

	private void WaitForPlayerJoined()
	{
		if (_joinCoroutine != null) return;
		DestroyPlayers();

		_joinCoroutine = StartCoroutine(WaitForJoin());
		return;

		IEnumerator WaitForJoin()
		{
			while (_canJoin)
			{
				if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
				{
					TryJoinPlayer(Keyboard.current);
				}

				List<Gamepad> joiningPlayers = new List<Gamepad>();
				foreach (Gamepad gamepad in Gamepad.all)
				{
					foreach (InputControl control in gamepad.allControls)
					{
						if (control is ButtonControl buttonControl)
						{
							if (buttonControl.isPressed && !buttonControl.synthetic)
							{
								joiningPlayers.Add(gamepad);
								break;
							}
						}
					}
				}

				foreach (Gamepad gamepad in joiningPlayers)
				{
					TryJoinPlayer(gamepad);
				}
				// foreach (Gamepad gamepad in Gamepad.all.Where(gamepad => gamepad.buttonSouth.wasPressedThisFrame))
				// {
				// 	TryJoinPlayer(gamepad);
				// }
				yield return null;
			}
			_joinCoroutine = null;
		}
	}

	private void TryJoinPlayer(InputDevice device)
	{
		if (PlayerInput.FindFirstPairedToDevice(device) != null)  return;
		_inputManager.playerPrefab = Game.Instance.PlayerPrefab;
		_inputManager.JoinPlayer(
			playerIndex: joinedPlayerCount,
			pairWithDevice: device
		);
	}
	
	public void OnPlayerJoined(PlayerInput playerInput)
	{
		PlayerController player = playerInput.GetComponent<PlayerController>();
		player.Register(playerInput, OnPlayerDied);
		_players[playerInput.playerIndex] = new PlayerTrack(player);
		Joined?.Invoke(player);
		joinedPlayerCount++;
		
	}

	public void OnEndJoin()
	{
		int totalPlayers = _players.Count(t => t.controller != null);
		_activePlayers = new PlayerTrack[totalPlayers];
		for (int i = 0; i < totalPlayers; i++)
		{
			_activePlayers[i] = _players[i];
		}
		JoinEnded?.Invoke();
	}
	
	#endregion
	
	#region Player Spawning
	
	public void SpawnPlayers()
	{
		foreach (PlayerTrack tracker in _activePlayers)
		{
			if (tracker.isDead) continue;
			LookForPlayerSpawn(tracker.controller.ActivePlayer);
		}
	}
	
	public static void LookForPlayerSpawn(PlayerBase player)
	{
		PlayerSpawn[] spawns = FindObjectsByType<PlayerSpawn>(FindObjectsSortMode.None);
		foreach (PlayerSpawn spawn in spawns)
		{
			if (player.PlayerIndex == (int)spawn.Type)
			{
				spawn.Spawn(player);
			}
		}
	}
	#endregion
	
	public void OnPlayerDied(Player player)
	{
		player.FreezePlayer();
		player.transform.position = _playerJail.position;

		switch (PlayingState.CurrentGameplayState)
		{
			case GameplayStates.Combat:
			{
				_players[player.PlayerIndex].isDead = true;
			
				PlayerTrack[] alive = _alivePlayers;
				switch (alive.Length - 1)
				{
					case 1:
						WinningPlayerIndex = alive[0].PlayerIndex;
						Game.currentState = GameStates.GameOver;
						break;
					case 0:
						Debug.LogError("zero players left should only happen if DEBUG!");
						break;
				}

				break;
			}
			case GameplayStates.MiniGame:
				try { _onPlayerDiedInMinigame.Invoke(player); }
				catch (NullReferenceException) 
				{
					//the code doesn't know what to do when a player dies
					Debug.LogError("No Mini-Game death event attached! Make sure there is a MiniGameInfo object in the scene");
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void SetPlayerStates(PlayerState state)
	{
		foreach (PlayerTrack tracker in _activePlayers)
		{
			tracker.controller.CurrentState = state;
		}
	}

    //used for unfreezing all players after minigame
    public void UnfreezeAllPlayers()
    {
        foreach (PlayerTrack tracker in _players)
        {
            if (!tracker.isDead)
            {
                //tracker.player.UnfreezePlayer();
            }
        }
    }
	
	public void DestroyPlayers()
	{
		joinedPlayerCount = 0;
		WinningPlayerIndex = -1;
		foreach (PlayerTrack tracker in _activePlayers)
		{
			Destroy(tracker.controller.gameObject);
		}
		_players = new PlayerTrack[MAX_PLAYER];
		_activePlayers = Array.Empty<PlayerTrack>();
		GameCanvas.Instance.DestroyUI();
	}

	public static void DEBUG_SetPlayerStates(PlayerState state)
	{
		foreach (PlayerController controller in Game.GetPlayers())
		{
			controller.CurrentState = state;
		}
	}
    
    private void ResetPlayerAfterMinigame(Player player)
    {
        //_players[player.PlayerIndex].isAliveInMinigame = true;
    }

    public void ResetPlayerHealth()
    {
	    
    }
}


	//TODO
	// private void CreatePlayerUI(PlayerInput playerInput)
	// {
		// GameObject uiInstance = Instantiate(_playerUIPrefab, _playerUIGroup);
		// PlayerUIDisplayer playerUIDisplayer = uiInstance.GetComponent<PlayerUIDisplayer>();
	 //   
		// // Connect hurtbox to UI
		// playerUIDisplayer.InitializePlayerUI(playerInput.GetComponentInChildren<AttackHurtbox>());
	 //   
		// // Connect tracker to player
		// UltimateAttackTracker tracker = playerInput.GetComponent<UltimateAttackTracker>();
	 //   
		// if (tracker != null && playerUIDisplayer != null)
		// {
		// 	//connect tracker to UI 
		// 	tracker.SetPlayerUI(playerUIDisplayer);
		// 	//let the UI know when the ultimate attack is unlocked
		// 	playerUIDisplayer.SubscribeToTracker(tracker);
		// 	Debug.Log("Successfully connected tracker to UI");
		// }
		// else
		// {
		// 	Debug.LogError("Failed to find tracker or UI displayer");
		// }
	// }
