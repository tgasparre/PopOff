using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IActivePlayerTracker
{
	public void SpawnPlayers();
	public void DestroyPlayers();
	public void SetPlayerStates(PlayerState state);
	public PlayerController[] GetPlayers();
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

	public static Action JoinEnded;
	public static Action<PlayerController> Joined;
	
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

				foreach (var gamepad in Gamepad.all.Where(gamepad => gamepad.buttonSouth.wasPressedThisFrame))
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
			playerIndex: _activePlayers.Length,
			pairWithDevice: device
		);
	}
	
	public void OnPlayerJoined(PlayerInput playerInput)
	{
		PlayerController player = playerInput.GetComponent<PlayerController>();
		player.Register(playerInput, OnPlayerDied);
		_players[playerInput.playerIndex] = new PlayerTrack(player);
		Joined?.Invoke(player);
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
		if (PlayingState.CurrentGameplayState == GameplayStates.Combat)
		{
			_players[player.PlayerIndex].isDead = true;
		}

		PlayerTrack[] alive = _alivePlayers;
		switch (alive.Length)
		{
			case 1:
				WinningPlayerIndex = alive[0].PlayerIndex;
				Game.currentState = GameStates.GameOver;
				break;
			case 0:
				Debug.LogError("zero players left should only happen if DEBUG!");
				break;
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
		WinningPlayerIndex = -1;
		foreach (PlayerTrack tracker in _activePlayers)
		{
			Destroy(tracker.controller.gameObject);
		}
		_players = new PlayerTrack[MAX_PLAYER];
		_activePlayers = Array.Empty<PlayerTrack>();
	}

	public static void DEBUG_SetPlayerStates(PlayerState state)
	{
		foreach (PlayerController controller in Game.GetPlayers())
		{
			controller.CurrentState = state;
		}
	}
}



	//subscribes to PlayerDied event so the player will tell us when it drops to 0 hp
	//also adds player to our player list and makes sure they have the correct input manager
	// private void RegisterPlayer(Player player)
	// {
	// 	_players.Add(player);
	// 	player.PlayerDied += OnPlayerDied;
	//     
	// }

	// private void RespawnPlayer()
	// {
	// 	
	// }
	
	// private void OnPlayerDied()
	// {
	// 	--alivePlayers;
	// 	if (alivePlayers <= 1)
	// 	{
	// 		playerWon?.Invoke();
	// 	}
	// }

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
	
	//    public event Action playerWon;
	//    
	//    [SerializeField] private GameObject playerUIPrefab;
	//    [SerializeField] private Transform uiLayoutGroup;
	//    
	//    [SerializeField] private Transform[] spawnPoints;
	//    [SerializeField] private GameObject playerPrefab;
	//    private int numPlayersAlive;
	//    private Player winningPlayer;
	//    private List<Player> players = new List<Player>();
	//    
	//    [SerializeField] private PlayerInputManager inputManager;
	//
	//    private void Awake() {
	//        // Subscribe to the player joined event
	//        inputManager.onPlayerJoined += OnPlayerJoined;
	//    }
	//
	//    private void OnPlayerJoined(PlayerInput playerInput) 
	// {
	//        Debug.Log("OnPlayerJoined was called");
	//        
	//        Player playerScript = playerInput.GetComponent<Player>();
	//        RegisterPlayer(playerScript);
	//        
	//        // Spawn UI
	//        GameObject uiInstance = Instantiate(playerUIPrefab, uiLayoutGroup);
	//        PlayerUIDisplayer playerUIDisplayer = uiInstance.GetComponent<PlayerUIDisplayer>();
	//        
	//        // Connect hurtbox to UI
	//        playerUIDisplayer.InitializePlayerUI(playerInput.GetComponentInChildren<AttackHurtbox>());
	//        
	//        // Connect tracker to player
	//        UltimateAttackTracker tracker = playerInput.GetComponent<UltimateAttackTracker>();
	//        
	//        if (tracker != null && playerUIDisplayer != null)
	//        {
	// 		//connect tracker to UI 
	//            tracker.SetPlayerUI(playerUIDisplayer);
	//            //let the UI know when the ultimate attack is unlocked
	//            playerUIDisplayer.SubscribeToTracker(tracker);
	//            Debug.Log("Successfully connected tracker to UI");
	//        }
	//        else
	//        {
	//            Debug.LogError("Failed to find tracker or UI displayer");
	//        }
	//    }
	//
	//    public string GetWinnerName()
	//    {
	//        return winningPlayer.playerStats.PlayerName;
	//    }
	//
	//    public void SetNumPlayersAlive(int numPlayers)
	//    {
	//        numPlayersAlive = numPlayers;
	//    }
	//
	//    public int GetNumPlayersAlive()
	//    {
	//        return numPlayersAlive;
	//    }
	//
	//    public void BlockPlayerMovement()
	//    {
	//        foreach (Player player in players)
	//        {
	//            player.FreezePlayerMovement();
	//        }
	//    }
	//
	//    public void UnblockPlayerMovement()
	//    {
	//        foreach (Player player in players)
	//        {
	//            player.UnfreezePlayerMovement();
	//        }
	//    }
	//
	//    public void ResetAllPlayerTrackers()
	//    {
	//        foreach (Player player in players)
	//        {
	//            player?.gameObject.GetComponent<UltimateAttackTracker>().ResetTracker();
	//        }
	//    }
	//    
	// private void OnPlayerDied()
	// {
	//     --numPlayersAlive;
	//     if (numPlayersAlive <= 1)
	//     {
	//         playerWon?.Invoke();
	//     }
	// }
	//
	// //subscribes to PlayerDied event so the player will tell us when it drops to 0 hp
	// //also adds player to our player list and makes sure they have the correct input manager
	// private void RegisterPlayer(Player player)
	// {
	//     players.Add(player);
	//     player.PlayerDied += OnPlayerDied;
	//     
	// }
	//
	//    //unsubscribe to PlayerDied for each player to prevent memory leaks
	//    private void OnDestroy()
	//    { 
	//        foreach (Player player in players) 
	//        {
	//            if (player != null)
	//            {
	//                player.PlayerDied -= OnPlayerDied;
	//            }
	//        }
	//        
	//        if (inputManager != null) {
	//            inputManager.onPlayerJoined -= OnPlayerJoined;
	//        }
	//    }
