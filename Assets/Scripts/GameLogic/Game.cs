using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _sceneLoader = GetComponent<SceneLoader>();
        _activePlayersTracker = GetComponentInChildren<ActivePlayersTracker>();
        currentState = GameStates.Menu;
    }
    
    [Header("Player Prefabs")]
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _startingPrefab;
    public bool useStartingPlayers = true;
    public GameObject PlayerPrefab => (useStartingPlayers) ? _startingPrefab : _playerPrefab;

    public static GameStates currentState
    {
        get => StateMachineManager.currentState;
        set => StateMachineManager.SwitchState(value);
    }

    private SceneLoader _sceneLoader;
    public ISceneLoader SceneLoader => _sceneLoader;
    private ActivePlayersTracker _activePlayersTracker;
    public bool CanJoin
    {
        get => _activePlayersTracker.CanJoin;
        set => _activePlayersTracker.CanJoin = value;
    }
    public bool CanLoadScene => _sceneLoader.canLoadScene;
    public int PlayerCount => _activePlayersTracker.activePlayers;
    public int WinningIndex => _activePlayersTracker.winningPlayerIndex;
    
    public static void ExitGame()
    {
        Application.Quit();
    }

    public void SpawnPlayers()
    {
        _activePlayersTracker.SpawnPlayers();
    }
    public void DestroyPlayers()
    {
        _activePlayersTracker.DestroyPlayers();
    }

    public void OnPlayerDied(Player player)
    {
        _activePlayersTracker.OnPlayerDied(player);
    }
    
    // public GameTools gameTools = new GameTools();
    //
    // [SerializeField] private StateMachineManager gameStateManager;
    // [SerializeField] private ActivePlayersTracker activePlayersTracker;
    //
    //
    // public static Game Instance { get; private set; }
    //
    // private void Awake() 
    // { 
    //     if (Instance != null && Instance != this) 
    //     { 
    //         Destroy(this); 
    //     } 
    //     else 
    //     { 
    //         Instance = this; 
    //     }
    //     
    //     //subscribes to PlayerWon event (player tracker tells game when one player is left alive)
    //     if (activePlayersTracker != null)
    //     {
    //         activePlayersTracker.playerWon += OnPlayerWon;
    //     }
    // }
    //
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // { 
    //     ShowStartMenus();
    // }
    //
    // // Update is called once per frame
    // void Update()
    // {
    //
    // }
    //
    // public void Reset()
    // {
    //     // go back to title screen or character select screen
    //     // stop all movement/functions/coroutines happening
    //     gameStateManager.EnterPreStartState();
    // }
    //
    // public void StartGame()
    // {
    //     // move to player number select, then start minigame
    //     // maybe add another method here to proc player select, then in that method move to minigame state
    //     //spawn in players into main fighting arena
    //     gameStateManager.EnterPVPCombatState();
    // }
    //
    // public void PauseGame()
    // {
    //     gameStateManager.EnterPauseState();
    // }
    //
    // public void ResumeGame()
    // {
    //     //will have to change this to be able to pause from minigame state
    //     gameStateManager.EnterPVPCombatState();
    // }
    //
    // public void EndGame()
    // {
    //     //TODO: edit this to display name of which player won
    //     gameStateManager.EnterGameOverState();
    // }
    //
    // public void CloseGame()
    // { 
    //     Application.Quit();
    //     #if UNITY_EDITOR
    //         UnityEditor.EditorApplication.isPlaying = false;
    //     #endif
    //     
    // }
    //
    // public void TriggerMinigame()
    // {
    //     gameStateManager.EnterMiniGameState();
    // }
    //
    // public void EndMinigame()
    // {
    //     gameStateManager.EnterPVPCombatState();
    // }
    //
    // private void ShowStartMenus()
    // {
    //     if (!gameTools.HasGameStarted())
    //     {
    //         gameStateManager.EnterPreStartState();
    //         gameTools.SetGameStarted(true);
    //     }
    // }
    //
    // private void OnPlayerWon()
    // {
    //     activePlayersTracker.ResetAllPlayerTrackers();
    //     gameStateManager.EnterGameOverState();
    //     //display name of player thats remaining
    // }
    //
    // private void OnDestroy()
    // {
    //     //unsubscribe from playerWon to prevent memory leaks
    //     if (activePlayersTracker != null)
    //     {
    //         activePlayersTracker.playerWon -= OnPlayerWon;
    //     }
    // }

}
