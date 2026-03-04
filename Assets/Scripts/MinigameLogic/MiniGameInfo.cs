using System;
using System.Collections;
using UnityEngine;

public abstract class MiniGameInfo : MonoBehaviour
{
    public static MiniGameInfo Instance;
    public static bool IsPlayingMiniGame => Instance._isPlayingMiniGame;
    protected void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    private void OnDestroy()
    {
        Instance = null;
    }

    [SerializeField] private string _miniGameName = "default name";
    public string MiniGameName => _miniGameName;
    [SerializeField] [TextArea] private string _miniGameInstructions = "default description";
    public string MiniGameInstructions => _miniGameInstructions;
    [Space(2)] [SerializeField] private int _countdownTimer = 3;
    public int CountdownTimer => _countdownTimer;
    [SerializeField] private int _miniGameTime = 15;
    public int MiniGameTime => _miniGameTime;
    public bool HasTimer => _miniGameTime > 0;
    
    [Space]
    [Tooltip("Time to wait before starting the countdown after loading the scene")] [SerializeField] private float _waitAfterLoadingTime = 0.8f;
    [Tooltip("Time to read the instructions of the minigame before starting the countdown")] [SerializeField] private float _waitForInstructionTime = 4f;
    [Tooltip("Time to wait before actually beginning the minigame")] [SerializeField] private float _waitBeforeStartingTime = 0.5f;
    [Tooltip("Time to wait after the game is over before going to the next scene")] [SerializeField] private float _waitBeforeSceneLoad = 1f;

    private bool _isPlayingMiniGame = false;
    protected PlayerController[] _players;
    private float _resultsTime = 1.3f;

    private int _alivePlayers;

    private Action _onGameComplete;
    private Coroutine _countdownCoroutine;

    /// <summary>
    /// Play the intro animation (display title, countdown to start)
    /// </summary>
    public void Intro(Action onIntroComplete, Action onGameComplete)
    {
        _alivePlayers = Game.PlayerCount; 
        _onGameComplete = onGameComplete;
        StartCoroutine(IntroCountdown());
        return;
        
        IEnumerator IntroCountdown()
        {
            yield return new WaitForSeconds(_waitAfterLoadingTime);
            yield return new WaitForSeconds(_waitForInstructionTime);
            GameCanvas.Instance.HideMiniGameDescription();
            yield return new WaitForSeconds(0.6f);
            yield return StartCoroutine(Countdown(_countdownTimer));
            yield return new WaitForSeconds(_waitBeforeStartingTime);
            
            onIntroComplete.Invoke();
            if (!HasTimer) yield break;
            
            //being minigame timer
            _countdownCoroutine = StartCoroutine(Countdown(_miniGameTime, _onGameComplete));
        }
    }

    private static IEnumerator Countdown(float startTime, Action finished = null)
    {
        float timer = startTime;
        while (timer >= 0)
        {
            GameCanvas.Instance.UpdateMiniGameCountdown(Mathf.RoundToInt(timer).ToString());    
            timer -= Time.deltaTime;
            yield return null;
        }
        finished?.Invoke();
    }
    
    public void Begin(PlayerController[] players)
    {
        _players = players;
        _isPlayingMiniGame = true;
        StartMiniGame();
    }

    public void ShowResults(Action onDelayOver, Action onFinished)
    {
        _isPlayingMiniGame = false;
        if (_countdownCoroutine != null)
        {
            StopCoroutine(_countdownCoroutine);
            _countdownCoroutine = null;
        }
        StartCoroutine(SmallDelay());
        return;

        IEnumerator SmallDelay()
        {
            yield return new WaitForSeconds(_resultsTime);
            onDelayOver.Invoke();
            yield return new WaitForSeconds(0.2f);
            ShowMiniGameResults(onFinished);
        }
    }

    public void End()
    {
        _players = null;
        StartCoroutine(WaitForSceneLoad());
        return;

        IEnumerator WaitForSceneLoad()
        {
            yield return new WaitForSeconds(_waitBeforeSceneLoad);
            PlayingState.CurrentGameplayState = GameplayStates.Combat;
        }
    }

    /// <summary>
    /// calls the end mini-game section early, before the timer expires 
    /// </summary>
    public void TriggerEndMiniGame()
    {
        _onGameComplete.Invoke();
    }

    /// <summary>
    /// called when a player dies in a mini-game, run TriggerEndMiniGame() if only one player is left 
    /// </summary>
    /// <param name="player">player who lost</param>
    public void OnPlayerMiniGameLose(Player player)
    {
        _alivePlayers--;
        if (_alivePlayers <= 1) TriggerEndMiniGame();
    }

    protected abstract void StartMiniGame();
    protected abstract void ShowMiniGameResults(Action onFinished);
}
