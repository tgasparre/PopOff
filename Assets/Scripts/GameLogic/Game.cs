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
    public GameObject PlayerPrefab => _playerPrefab;

    private SceneLoader _sceneLoader;
    private ActivePlayersTracker _activePlayersTracker;
    
    public static GameStates currentState
    {
        get => StateMachineManager.currentState;
        set => StateMachineManager.SwitchState(value);
    }
    
    public static bool CanJoin
    {
        get => Instance._activePlayersTracker.CanJoin;
        set
        {
            Instance._activePlayersTracker.CanJoin = value;
            if (!value) Instance._activePlayersTracker.OnEndJoin();
        }
    }

    public static bool IsFrozen
    {
        get => Time.timeScale == 0f;
        set
        {
            Time.timeScale = value ? 0f : 1f;
            IsPlayersFrozen = value;
        }
    }

    public static bool IsPlayersFrozen
    {
        get => Instance._activePlayersTracker.IsPlayersFrozen;
        set => Instance._activePlayersTracker.IsPlayersFrozen = value;
    }
    
    public static bool CanLoadScene => Instance._sceneLoader.canLoadScene;
    public static int WinningIndex => Instance._activePlayersTracker.WinningPlayerIndex;
    public static PlayerController[] GetPlayers() {return Instance._activePlayersTracker.GetPlayers();}

    public static ISceneLoader SceneLoader => Instance._sceneLoader;
    public static IActivePlayerTracker ActivePlayerTracker => Instance._activePlayersTracker;
    
    
    public static void ExitGame()
    {
        Application.Quit();
    }
    
}
