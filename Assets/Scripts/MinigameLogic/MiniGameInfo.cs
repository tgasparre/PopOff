using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [SerializeField] private Powerup[] _powerupReward = Array.Empty<Powerup>();
    private Powerup _chosenPowerup;
    [SerializeField] private PlayerStats _minigameStats;
    public PlayerStats MiniGameStats => _minigameStats;
    [SerializeField] private int _miniGameStartingHealth = 100;
    
    [Space]
    [Tooltip("Time to wait before starting the countdown after loading the scene")] [SerializeField] private float _waitAfterLoadingTime = 0.8f;
    [Tooltip("Time to read the instructions of the minigame before starting the countdown")] [SerializeField] private float _waitForInstructionTime = 2f;
    [Tooltip("Time to wait before actually beginning the minigame")] [SerializeField] private float _waitBeforeStartingTime = 0.5f;
    [Tooltip("Time to wait after the game is over before going to the next scene")] [SerializeField] private float _waitBeforeSceneLoad = 1f;

    private PlayerTrack[] _players;
    private bool _isPlayingMiniGame = false;
    private float _resultsTime = 1.3f;

    protected PlayerController[] _playerControllers;
    protected int _alivePlayers;
    protected int _winningPlayerIndex = -1;
    protected float[] _combatPlayerHealth;

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
            if (!HasTimer)
            {
                GameCanvas.Instance.UpdateMiniGameCountdown("");
                yield break;
            }
            
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
        _players = new PlayerTrack[players.Length];
        foreach (PlayerController controller in players)
        {
            _players[controller.PlayerIndex] = new PlayerTrack(controller, controller.PlayerHealth);
            controller.PlayerHealth = _miniGameStartingHealth;
        }
        _playerControllers = _players.Select(t => t.controller).ToArray();
        AssignWeightClasses(_minigameStats);
        
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
            yield return new WaitForSecondsRealtime(_resultsTime);
            onDelayOver.Invoke();
            yield return new WaitForSecondsRealtime(0.2f);
            _chosenPowerup = GetRandomPowerup();
            ShowMiniGameResults(onFinished, _chosenPowerup ? _chosenPowerup.Name : "");
        }
    }

    public void End()
    {
        //apply powerup
        if (_chosenPowerup != null)
        {
            _players[_winningPlayerIndex].controller.ApplyPowerup(_chosenPowerup);
        }

        //reset health
        foreach (PlayerTrack track in _players)
        {
            track.controller.PlayerHealth = track.gameHealth;
        }
        
        ResetWeightClasses();
        _players = null;
        _playerControllers = null;
        _winningPlayerIndex = -1;
        _chosenPowerup = null;
        StartCoroutine(WaitForSceneLoad());
        return;

        IEnumerator WaitForSceneLoad()
        {
            yield return new WaitForSecondsRealtime(_waitBeforeSceneLoad);
            PlayingState.CurrentGameplayState = GameplayStates.Combat;
        }
    }

    private Powerup GetRandomPowerup()
    {
        return _powerupReward.Length == 0 ? null : _powerupReward[Random.Range(0, _powerupReward.Length)];
    }

    private void AssignWeightClasses(PlayerStats stats)
    {
        if (stats == null) return;
        foreach (PlayerController controller in _playerControllers)
        {
            if (controller.ActivePlayer is Player player)
            {
                player.AssignWeightClass(stats);
            }
        }
    }

    private void ResetWeightClasses()
    {
        foreach (PlayerController controller in _playerControllers)
        {
            if (controller.ActivePlayer is Player player)
            {
                player.ResetWeightClass();
            }
        }
    }

    /// <summary>
    /// calls the end mini-game section early, before the timer expires 
    /// </summary>
    public void TriggerEndMiniGame(int winningIndex)
    {
        _winningPlayerIndex = winningIndex;
        _onGameComplete.Invoke();
    }

    /// <summary>
    /// called when a player dies in a mini-game, runs TriggerEndMiniGame() if only one player is left 
    /// </summary>
    /// <param name="player">player who lost</param>
    public void OnPlayerMiniGameLose(Player player)
    {
        _alivePlayers--;
        _players[player.PlayerIndex].isDeadInMiniGame = true;

        if (_alivePlayers <= 1)
        {
            //find winner
            foreach (PlayerTrack track in _players)
            {
                if (!track.isDeadInMiniGame)
                {
                    TriggerEndMiniGame(track.PlayerIndex);
                    return;
                }
            }
            Debug.LogError("no alive player was found, an error");
        }
    }

    protected abstract void StartMiniGame();

    protected virtual void ShowMiniGameResults(Action onFinished, string reward)
    {
        Game.IsFrozen = true;
        GameCanvas.Instance.OnWinMiniGame(_winningPlayerIndex, reward);
        StartCoroutine(ResultsScreen());
        return;
        
        IEnumerator ResultsScreen()
        {
            yield return new WaitForSecondsRealtime(2f);
            onFinished.Invoke();
        }
    }
    
    protected struct PlayerTrack
    {
        public readonly PlayerController controller;
        public bool isDeadInMiniGame;
        public float gameHealth;
        public int PlayerIndex => controller.PlayerIndex;

        public PlayerTrack(PlayerController c, float hp)
        {
            controller = c;
            isDeadInMiniGame = false;
            gameHealth = hp;
        }
    }
}
