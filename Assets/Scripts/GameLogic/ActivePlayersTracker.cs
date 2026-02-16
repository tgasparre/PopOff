using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivePlayersTracker : MonoBehaviour
{
	// [SerializeField] private Transform _playerUIGroup;
	// [SerializeField] private GameObject _playerUIPrefab;
	private PlayerInputManager _inputManager;
	private List<PlayerInput> _players = new List<PlayerInput>();
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
	
	public int activePlayers => _players.Count;
	public int alivePlayers { get; private set; }

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

				foreach (Gamepad gamepad in Gamepad.all)
				{
					if (gamepad.buttonSouth.wasPressedThisFrame)
					{
						TryJoinPlayer(gamepad);
					}
				}
				yield return null;
			}
			_joinCoroutine = null;
		}
	}

	private void TryJoinPlayer(InputDevice device)
	{
		if (PlayerInput.FindFirstPairedToDevice(device) != null)  return;
		_inputManager.JoinPlayer(
			playerIndex: -1,
			splitScreenIndex: -1,
			controlScheme: null,
			pairWithDevice: device
		);
	}
	
	public void OnPlayerJoined(PlayerInput playerInput) 
	{
		_players.Add(playerInput);
		DontDestroyOnLoad(playerInput.gameObject);
		LookForPlayerSpawn(playerInput);
		
		// RegisterPlayer(playerScript);
		// CreatePlayerUI(playerInput);
	}
	
	#endregion
	
	#region Player Spawning
	
	public void SpawnPlayers()
	{
		foreach (PlayerInput player in _players)
		{
			LookForPlayerSpawn(player);
		}
	}
	
	private void LookForPlayerSpawn(PlayerInput player)
	{
		PlayerSpawn[] spawns = FindObjectsByType<PlayerSpawn>(FindObjectsSortMode.None);
		foreach (PlayerSpawn spawn in spawns)
		{
			Player p = player.GetComponent<Player>();
			if (p.CanRespawn && player.playerIndex == (int)spawn.Type)
			{
				spawn.Spawn(player.gameObject); //todo -- doesn't work as expected
			}
		}
	}
	#endregion
	
	public bool OnPlayerDied(Player player)
	{
		player.FreezePlayer();
		player.transform.position = _playerJail.position;
		return PlayingState.CurrentGameplayState == GameplayStates.Combat;
	}

	public void SetPlayerMenu()
	{
		
	}
	
	public void DestroyPlayers()
	{
		foreach (PlayerInput p in _players)
		{
			Destroy(p.gameObject);
		}
		_players.Clear();
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
	private void CreatePlayerUI(PlayerInput playerInput)
	{
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
	}
	
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
}
