using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class MiniGameInfo : MonoBehaviour
{
    public static MiniGameInfo Instance;
    public static bool IsPlayingMiniGame => Instance._isPlayingMiniGame;

    public ParticleSystem winningParticlesPrefab;
    private ParticleSystem _winningParticles;
    
    protected void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    protected void OnDestroy()
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
    [SerializeField] private int _miniGameStartingHealth = 200;
    
    [Space]
    [Tooltip("Time to wait before starting the countdown after loading the scene")] [SerializeField] private float _waitAfterLoadingTime = 0.8f;
    [Tooltip("Time to read the instructions of the minigame before starting the countdown")] [SerializeField] private float _waitForInstructionTime = 2f;
    [Tooltip("Time to wait after the game is over before going to the next scene")] [SerializeField] private float _waitBeforeSceneLoad = 1f;

    private Dictionary<int, PlayerTrack> _players;
    private float _resultsTime = 2f;

    protected bool _isPlayingMiniGame { get; private set; }  = false;
    protected PlayerController[] _playerControllers;
    protected int _alivePlayers;
    protected int _winningPlayerIndex = -1;

    private Action _onGameComplete;
    private Coroutine _countdownCoroutine;

    /// <summary>
    /// Play the intro animation (display title, countdown to start)
    /// </summary>
    public void Intro(Action onIntroComplete, Action onGameComplete, PlayerController[] alivePlayers)
    {
        _countdownCoroutine = null;
        _players = new Dictionary<int, PlayerTrack>(alivePlayers.Length);
        foreach (PlayerController controller in alivePlayers)
        {
            _players[controller.PlayerIndex] = new PlayerTrack(controller, controller.PlayerHealth);
            controller.PlayerHealth = _miniGameStartingHealth;
        }
        _playerControllers = _players.Select(t => t.Value.controller).ToArray();
        AssignWeightClasses(_minigameStats);

        _alivePlayers = alivePlayers.Length;
        _onGameComplete = onGameComplete;
        StartCoroutine(StartCountdown());
        return;
        
        IEnumerator StartCountdown()
        {
            yield return new WaitForSeconds(_waitAfterLoadingTime + _waitForInstructionTime);
            GameCanvas.Instance.HideMiniGameDescription();

            yield return GameCanvas.MiniGameUI.StartCurrentCountdown();

            onIntroComplete.Invoke();
            if (!HasTimer)
            {
                GameCanvas.MiniGameUI.DisableCountdown();
                yield break;
            }
            
            //being minigame timer
            _countdownCoroutine = GameCanvas.MiniGameUI.StartCurrentCountdown(_onGameComplete);
        }
    }
    
    public void Begin()
    {
        _isPlayingMiniGame = true;
        StartMiniGame();
    }

    public void ShowResults(Action onDelayOver, Action onFinished)
    {
        AudioManager.PlaySound(AudioTrack.MinigameWhistle);
        
        _isPlayingMiniGame = false;
        OnEndMiniGame();
        
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
            yield return new WaitForSecondsRealtime(0.25f);
            
            AudioManager.PlaySound(AudioTrack.MinigameEnd);
            _chosenPowerup = GetRandomPowerup();
            ShowMiniGameResults(onFinished, _chosenPowerup);
        }
    }

    public void End()
    {
        //apply powerup
        if (_chosenPowerup != null && _winningPlayerIndex is >= 0 and < 4)
        {
            _players[_winningPlayerIndex].controller.ApplyPowerup(_chosenPowerup);
        }

        //reset health
        foreach (KeyValuePair<int, PlayerTrack> track in _players)
        {
            if (track.Value.gameHealth == 0) continue;
            track.Value.controller.PlayerHealth = track.Value.gameHealth;
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
        
        if (_winningPlayerIndex is >= 0 and < 4)
            SpawnWinningParticles(_players[_winningPlayerIndex].controller.PlayerHurtbox.gameObject.transform.position);
        
        _onGameComplete.Invoke();
    }

    /// <summary>
    /// called when a player dies in a mini-game, runs TriggerEndMiniGame() if only one player is left 
    /// </summary>
    /// <param name="player">player who lost</param>
    public void OnPlayerMiniGameDie(Player player)
    {
        _alivePlayers--;
        _players[player.PlayerIndex].isDeadInMiniGame = true;

        if (_alivePlayers <= 1)
        {
            //find winner
            foreach (KeyValuePair<int, PlayerTrack> track in _players)
            {
                if (!track.Value.isDeadInMiniGame)
                {
                    TriggerEndMiniGame(track.Value.PlayerIndex);
                    return;
                }
            }
            Debug.LogWarning("no alive player was found, if not DEBUG then we have a problem");
            TriggerEndMiniGame(_players[0].PlayerIndex); //set player one to win by default 
        }
    }

    private void SpawnWinningParticles(Vector3 position)
    {
        if (winningParticlesPrefab != null)
            _winningParticles = Instantiate(winningParticlesPrefab, position,quaternion.identity);
    }

    protected abstract void StartMiniGame();
    protected virtual void OnEndMiniGame() { /* to be inherited */}

    protected virtual void ShowMiniGameResults(Action onFinished, Powerup reward)
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

    //called when exiting the state -- disables all minigame related UI and timers
    public void ForceEnd()
    {
        if (!_isPlayingMiniGame) return;
        GameCanvas.MiniGameUI.StopCurrentCountdownNoTrigger();
        _countdownCoroutine = null;
    }
    
    protected class PlayerTrack
    {
        public readonly PlayerController controller;
        public bool isDeadInMiniGame;
        public readonly int gameHealth;
        public int PlayerIndex => controller.PlayerIndex;

        public PlayerTrack(PlayerController c, int hp)
        {
            controller = c;
            isDeadInMiniGame = false;
            gameHealth = hp;
        }
    }
}
