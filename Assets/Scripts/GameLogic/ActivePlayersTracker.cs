using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public interface IActivePlayerTracker
{
	public void SpawnAllPlayers();
	public void DestroyPlayers();
	public void SetPlayerStates(PlayerState state);
	public PlayerController[] GetPlayers();
	public PlayerController[] GetAlivePlayers();
	public void ResetMinigameDeaths();
	public event Action<int> OnPlayerFinishMinigame;
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

	private class PlayerTrack
	{
		public PlayerController controller;
		public bool isDead;
		public bool isDeadInMinigame;
		public int PlayerIndex => controller.PlayerIndex;

		public PlayerTrack(PlayerController c)
		{
			controller = c;
			isDead = false;
			isDeadInMinigame = false;
		}
	}

	private PlayerTrack[] _players = new PlayerTrack[MAX_PLAYER]; //DO NOT USE TO REFERENCE PLAYERS!
	
	private PlayerTrack[] _activePlayers = Array.Empty<PlayerTrack>(); //REFERENCE PLAYERS IN THE GAME
	private PlayerTrack[] _alivePlayers => _activePlayers.Where(t => !t.isDead).ToArray();
	private PlayerTrack[] _aliveMinigamePlayers => _alivePlayers.Where(t => !t.isDeadInMinigame).ToArray();
	public PlayerController[] GetPlayers() => _activePlayers.Select(tracker => tracker.controller).ToArray();
	public PlayerController[] GetAlivePlayers() => _alivePlayers.Select(tracker => tracker.controller).ToArray();
	
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
	
	public event Action<int> OnPlayerFinishMinigame;

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
		joinedPlayerCount++;
		Joined?.Invoke(player);
		
	}

	public void OnEndJoin()
	{
		int totalPlayers = _players.Count(t => t != null);
		_activePlayers = new PlayerTrack[totalPlayers];
		for (int i = 0; i < totalPlayers; i++)
		{
			_activePlayers[i] = _players[i];
		}
		JoinEnded?.Invoke();
	}
	
	#endregion
	
	#region Player Spawning
	
	public void SpawnAllPlayers()
	{
		PlayerSpawner spawner = FindFirstObjectByType<PlayerSpawner>();
		spawner.SpawnPlayers(GetAlivePlayers().Select(controller => controller.ActivePlayer).ToArray());
	}
	
	public static void SpawnSinglePlayer(PlayerBase player)
	{
		PlayerSpawner spawner = FindFirstObjectByType<PlayerSpawner>();
		spawner.SpawnPlayer(player);
	}
	#endregion
	
	public void OnPlayerDied(Player player)
	{
		player.FreezePlayer();
		player.transform.position = _playerJail.position;
		player.ResetHasDied();
		
		//if the players are frozen don't kill them something is loading and they can't move
		if (Game.IsPlayersFrozen) return; 

		switch (PlayingState.CurrentGameplayState)
		{
			case GameplayStates.Combat:
			{
				_activePlayers[player.PlayerIndex].isDead = true;
				PlayerTrack[] alive = _alivePlayers;
				switch (alive.Length)
				{
					case 1:
						WinningPlayerIndex = alive[0].PlayerIndex;
						Game.currentState = GameStates.GameOver;
						break;
					case 0:
						Debug.LogError("zero players left should only happen if DEBUG!");
						WinningPlayerIndex = 0;
						Game.currentState = GameStates.GameOver;
						break;
				}
				break;
			}
			case GameplayStates.MiniGame:
			{
				if (!MiniGameInfo.IsPlayingMiniGame) return; 
				
				_activePlayers[player.PlayerIndex].isDeadInMinigame = true;
				PlayerTrack[] alive = _aliveMinigamePlayers;
				switch (alive.Length)
				{
					case 1:
						OnPlayerFinishMinigame?.Invoke(alive[0].PlayerIndex);
						break;
					case 0:
						Debug.LogWarning("no alive player was found, if not DEBUG then we have a problem");
						OnPlayerFinishMinigame?.Invoke(_activePlayers[0].PlayerIndex); //set player one to win by default 
						break;
				}
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	public void ResetMinigameDeaths()
	{
		foreach (PlayerTrack tracker in _activePlayers)
		{
			tracker.isDeadInMinigame = false;
		}
	}

	public void SetPlayerStates(PlayerState state)
	{
		foreach (PlayerTrack tracker in _activePlayers)
		{
			tracker.controller.CurrentState = state;
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
}
