using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class StartingMiniGame : MiniGameInfo
{
    [Space]
    [SerializeField] private AudioSource _blowUpBallon;
    [SerializeField] private AudioManager.Audio _pressBlowButtonAudio;
    [Space]
    [SerializeField] private AirFillBoard[] _airFillBoards;
    [SerializeField] private WeightUI[] _weightUIs;
    [SerializeField] private float _fillRate = 0.1f;
    [SerializeField] private float _deflateRate = 0.2f;
    [Space]
    [SerializeField] private PlayerStats _lightClass;
    [SerializeField] private PlayerStats _defaultClass;
    [SerializeField] private PlayerStats _heavyClass;
    [Space] 
    [SerializeField] private ParticleSystem _airPoofParticlesPrefab;
    private ParticleSystem _airPoofParticles;

    private Coroutine _fillingCoroutine;
    private const float SFX_INTERVAL = 0.7f;

    private void SetMinigameUI(bool isVisible)
    {
        foreach (AirFillBoard board in _airFillBoards)
        {
            board.IsVisible = isVisible;
            board.SetValues(_fillRate, _deflateRate);
        }

        foreach (WeightUI weightUI in _weightUIs)
        {
            weightUI.IsVisible = isVisible;
        }
    }

    private void Start()
    {
        PauseState.OnPaused += OnPause;
        SetMinigameUI(false);
    }

    protected new void OnDestroy()
    {
        PauseState.OnPaused -= OnPause;
        base.OnDestroy();
    }

    private void OnPause(bool isPaused)
    {
        if (isPaused) _blowUpBallon.Pause();
        else _blowUpBallon.UnPause();
    }

    protected override void StartMiniGame()
    {
        //assign jump input to fill 
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            _airFillBoards[i].IsVisible = true;
            _playerControllers[i].OnJump = _airFillBoards[i].Fill;

            _weightUIs[i].IsVisible = true;
        }
        
        _blowUpBallon.PlayDelayed(1.2f);
    }

    protected override void ShowMiniGameResults(Action onFinished, Powerup reward)
    {
        //TODO fun animation
        for (int i = 0; i < _playerControllers.Length; i++)
        {
            _weightUIs[i].IsVisible = false;
            
            PlayerStats weightClass = _lightClass;
            Weight playerWeight = _airFillBoards[i].GetWeight();
            weightClass = playerWeight switch
            {
                Weight.Default => _defaultClass,
                Weight.Heavy => _heavyClass,
                _ => weightClass
            };
            
            _playerControllers[i].CurrentState = PlayerState.Fighting;
            ActivePlayersTracker.SpawnSinglePlayer(_playerControllers[i].ActivePlayer);
            if (_playerControllers[i].ActivePlayer is Player player)
            {
                player.AssignWeightClass(weightClass);
            }
            _airPoofParticles = Instantiate(_airPoofParticlesPrefab, _playerControllers[i].ActivePlayer.transform.position, Quaternion.identity);
        }
        onFinished.Invoke();
    }

    public void OnFill()
    {
        _fillingCoroutine ??= StartCoroutine(SFXDelay());
        return;
        
        IEnumerator SFXDelay()
        {
            AudioManager.PlaySound(_pressBlowButtonAudio);
            yield return new WaitForSeconds(SFX_INTERVAL);
            _fillingCoroutine = null;
        }
    }
    
    public enum Weight
    {
        Light,
        Default,
        Heavy
    }
}
