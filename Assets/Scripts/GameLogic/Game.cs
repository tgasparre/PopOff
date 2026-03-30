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
        _cameraShake = GetComponent<CameraShake>();
        currentState = GameStates.Menu;

        Cursor.visible = false;
    }
    
    [Header("Player Prefabs")]
    [SerializeField] private GameObject _playerPrefab;
    public GameObject PlayerPrefab => _playerPrefab;
    
    [Header("Player Animations")]
    [SerializeField] private RuntimeAnimatorController _mouseAnimation;
    [SerializeField] private RuntimeAnimatorController _dogAnimation;

    [SerializeField] private Color[] _playerColors;
    public Color[] PlayerColors => _playerColors;
    [SerializeField] private PlayerType[] _playerTypes;
    public PlayerType[] PlayerTypes => _playerTypes;
    
    public RuntimeAnimatorController GetPlayerAnimation(int index)
    {
        return _playerTypes[index] switch
        {
            PlayerType.Mouse => _mouseAnimation,
            PlayerType.Dog => _dogAnimation,
            _ => null
        };
    }

    private SceneLoader _sceneLoader;
    private ActivePlayersTracker _activePlayersTracker;
    private CameraShake _cameraShake;
    
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
    public static int PlayerCount => Instance._activePlayersTracker.joinedPlayerCount;

    public static ISceneLoader SceneLoader => Instance._sceneLoader;
    public static IActivePlayerTracker ActivePlayerTracker => Instance._activePlayersTracker;
    public static CameraShake CameraShake => Instance._cameraShake;
    
    public static void ExitGame()
    {
        CanJoin = false;
        IsFrozen = false;
        IsPlayersFrozen = false;
        Application.Quit();
    }

    [Header("DEBUG Controls")]
    [Tooltip("Make it so one player can start the game themselves")] public bool bypassOnePlayerBlock = true;
     
}

public enum PlayerType
{
    Mouse,
    Dog
}
