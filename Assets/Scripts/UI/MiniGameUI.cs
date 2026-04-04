using System;
using System.Collections;
using EasyTextEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public interface IMiniGameUI
{
    public MiniGameUI.UIState CurrentState { get; set; }
    public void SetValues(MiniGameInfo info);
    public Coroutine StartCurrentCountdown(Action onFinished = null);
    public void StopCurrentCountdownNoTrigger();
    public void DisableCountdown();
    public void DisableAll();
}
public class MiniGameUI : MonoBehaviour, IMiniGameUI
{
    [Header("Introduction State")]
    [SerializeField] private CanvasGroup _introCanvasGroup;
    [SerializeField] private TextMeshProUGUI _miniGameName;
    [SerializeField] private TextMeshProUGUI _miniGameDescription;
    [SerializeField] private ReadyGoCountdown _introCountdownTimer;
    
    [Header("MiniGame State")]
    [SerializeField] private CanvasGroup _miniGameCanvasGroup;
    [SerializeField] private TimerCountdown _miniGameCountdownTimer;
    
    [Header("Finished State")]
    [SerializeField] private CanvasGroup _finishedCanvasGroup;
    [SerializeField] private TextEffect _finishedEffect;
    
    [Header("Results State")]
    [SerializeField] private CanvasGroup _resultsCanvasGroup;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private Image _rewardImage;
    
    //===== State =====
    private UIState _currentState;
    private CountdownUI _currentCountdown;

    public UIState CurrentState
    {
        get => _currentState;
        set
        {
            DisableAll();
            switch (value)
            {
                case UIState.Introduction:
                    _currentCountdown = _introCountdownTimer;
                    _miniGameDescription.gameObject.SetActive(true);
                    _introCountdownTimer.gameObject.SetActive(false);
                    CanvasGroupDisplayer.Show(_introCanvasGroup);
                    break;
                case UIState.MiniGame:
                    _currentCountdown = _miniGameCountdownTimer;
                    CanvasGroupDisplayer.Show(_miniGameCanvasGroup);
                    break;
                case UIState.Finished:
                    _finishedEffect.StartManualEffects();
                    StartCoroutine(VerySmallDelay());
                    break;
                case UIState.Results:
                    CanvasGroupDisplayer.Show(_resultsCanvasGroup);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _currentState = value;
        }
    }

    private IEnumerator VerySmallDelay()
    {
        yield return new WaitForSeconds(.1f);
        CanvasGroupDisplayer.Show(_finishedCanvasGroup);
    }

    private void Awake()
    {
        CurrentState = UIState.Introduction;
        DisableAll();
    }

    public Coroutine StartCurrentCountdown(Action onFinished = null)
    {
       return _currentCountdown.StartCountdown(onFinished);
    }

    public void StopCurrentCountdownNoTrigger()
    {
        _currentCountdown.StopCountdownNoTrigger();
    }

    public void DisableCountdown()
    {
        _currentCountdown.StopCountdownNoTrigger();
    }

    public void HideDescription()
    {
        _miniGameDescription.gameObject.SetActive(false);
        _introCountdownTimer.gameObject.SetActive(true);
    }

    public void SetValues(MiniGameInfo info)
    {
        CurrentState = UIState.Introduction;
        _miniGameName.text = info.MiniGameName;
        _miniGameDescription.text = info.MiniGameInstructions;
        // _introCountdownTimer.InitializeCountdown(info.CountdownTimer);
        _miniGameCountdownTimer.InitializeCountdown(info.MiniGameTime);
    }

    public void OnWinMiniGame(int playerIndex, Powerup reward)
    {
        if (playerIndex is > 3 or < 0 || !reward) return;
        _playerName.text = GameUtils.PlayerNames[playerIndex];
        _rewardText.text = reward.Name;
        _rewardImage.sprite = reward.GetIcon();
        CurrentState = UIState.Results;
    }

    public void DisableAll()
    {
        CanvasGroupDisplayer.Hide(_finishedCanvasGroup);
        CanvasGroupDisplayer.Hide(_introCanvasGroup);
        CanvasGroupDisplayer.Hide(_miniGameCanvasGroup);
        CanvasGroupDisplayer.Hide(_resultsCanvasGroup);
        _currentCountdown = null;
    }

    public enum UIState
    {
        Introduction,
        MiniGame,
        Finished,
        Results
    }
}
