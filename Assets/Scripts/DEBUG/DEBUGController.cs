using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// DEBUG system for testing features in the game. Make sure this script isn't in the scene when building the game, it will break things
/// </summary>
public class DEBUGController : MonoBehaviour
{
    public static DEBUGController Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    [SerializeField] private int _playersToSpawn = 1;
    
    [Header("Toggles")]
    [SerializeField] private bool _disablePlayerUI = false;
    [SerializeField] private bool _playersImmortal = false;
    [SerializeField] private bool _playersOneHit = false;
    
    [Space] [SerializeField] private PlayerState _playerState = PlayerState.Fighting;
    [SerializeField] private GameStates _enteringState = GameStates.Playing; 
    public GameStates EnteringState => _enteringState;
    [SerializeField] [HideInInspector] private GameplayStates _playingState = GameplayStates.Combat;
    
    private IEnumerator Start()
    {
        StateMachineManager.DEBUG_SetGameState(_enteringState); 
        if (_enteringState == GameStates.Playing) PlayingState.DEBUG_SetGamePlayState(_playingState);
        ActivePlayersTracker.JoinEnded += JoinEnded;        
        ActivePlayersTracker.Joined += Joined;        
        Debug.Log("== Started DEBUG State: " + Game.currentState + " ==");
        
        Game.CanJoin = true;
        yield return new WaitUntil(() => Game.PlayerCount == _playersToSpawn);
        Game.CanJoin = false;
    }

    private void OnDestroy()
    {
        ActivePlayersTracker.JoinEnded -= JoinEnded;
        ActivePlayersTracker.Joined -= Joined;
    }

    private void OnValidate()
    {
        string activeName = SceneManager.GetActiveScene().name;
        switch (activeName)
        {
            case "Game":
                _enteringState = GameStates.Playing;
                _playingState = GameplayStates.Combat;
                break;
            case "Menu":
                _enteringState = GameStates.Menu;
                break;
            case "StartingMiniGame":
                _enteringState = GameStates.Playing;
                _playingState = GameplayStates.MiniGame;
                _playerState = PlayerState.StartingMiniGame;
                break;
            default:
                if (activeName.Contains("minigame", StringComparison.CurrentCultureIgnoreCase))
                {
                    _enteringState = GameStates.Playing;
                    _playingState = GameplayStates.MiniGame;
                }
                break;
        }
    }

    private void JoinEnded()
    {
        if (_playingState == GameplayStates.MiniGame) PlayingState.DEBUG_StartMiniGame();
        else if (_playingState == GameplayStates.Combat) PlayingState.DEBUG_StartCombatCountdown();
        ActivePlayersTracker.JoinEnded -= JoinEnded;
        ActivePlayersTracker.Joined -= Joined;
        
        if (_disablePlayerUI) FindFirstObjectByType<PlayerUIController>().gameObject.SetActive(false);
        if (_playersImmortal)
        {
            foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
            {
                player.PlayerHealth = 100000f;
            }
        }
        if (_playersOneHit)
        {
            foreach (Player player in FindObjectsByType<Player>(FindObjectsSortMode.None))
            {
                player.PlayerHealth = 1f;
            }
        }
    }

    private void Joined(PlayerController player)
    {
        player.CurrentState = _playerState;
        ActivePlayersTracker.LookForPlayerSpawn(player.ActivePlayer);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(DEBUGController))]
public class DEBUGEDITOR : Editor
{
    private SerializedProperty playingState;
    
    private void OnEnable()
    {
        playingState = serializedObject.FindProperty("_playingState");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DEBUGController controller = target as DEBUGController;
        
        EditorGUI.BeginChangeCheck();
        if (controller.EnteringState == GameStates.Playing)
        {
            EditorGUILayout.PropertyField(playingState);
        }
        if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
    }
}
#endif