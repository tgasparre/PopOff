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
    
    [SerializeField] private GameStates _enteringState = GameStates.Playing; 
    public GameStates EnteringState => _enteringState;

    [SerializeField] [HideInInspector] private GameplayStates _playingState = GameplayStates.Combat;
    public GameplayStates EnteringPlayingState => _playingState;

    [Space] [SerializeField] private bool useStartingPlayers = false;
    
    private void Start()
    {
        StateMachineManager.DEBUG_SetGameState(_enteringState); 
        if (_enteringState == GameStates.Playing) PlayingState.DEBUG_SetGamePlayState(_playingState);
        Debug.Log("== Started DEBUG State: " + Game.currentState + " ==");
        StartCoroutine(HandleDEBUGJoin());
    }

    private IEnumerator HandleDEBUGJoin()
    {
        // Game.Instance.useStartingPlayers = useStartingPlayers; 
        Game.Instance.CanJoin = true;
        yield return new WaitUntil(() => Game.Instance.PlayerCount == _playersToSpawn);
        Game.Instance.CanJoin = false;
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