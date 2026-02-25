using System;
using UnityEngine;
using Object = UnityEngine.Object;

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
    public int PlayerCount => _activePlayersTracker.activePlayerCount;
    public int WinningIndex => _activePlayersTracker.winningPlayerIndex;
    public PlayerController[] GetPlayers() {return _activePlayersTracker.Players;}
    
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

    public void SetPlayerStates(PlayerState state)
    {
        _activePlayersTracker.SetPlayerStates(state);
    }
    
    public void OnPlayerDied(Player player)
    {
        _activePlayersTracker.OnPlayerDied(player);
    }

    public static T GetFirstObjectFromType<T>() where T : Object
    {
        return FindFirstObjectByType<T>();
    }
}
