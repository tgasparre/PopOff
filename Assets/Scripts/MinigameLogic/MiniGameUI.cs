using System;
using System.Collections;
using TMPro;
using UnityEngine;

public interface IMiniGameUI
{
    public MiniGameUI.UIState CurrentState { get; set; }
    public void SetValues(MiniGameInfo info);
    public void DisableAll();
}
public class MiniGameUI : MonoBehaviour, IMiniGameUI
{
    [Header("Introduction State")]
    [SerializeField] private CanvasGroup _introCanvasGroup;
    [SerializeField] private TextMeshProUGUI _miniGameName;
    [SerializeField] private TextMeshProUGUI _miniGameDescription;
    [SerializeField] private TextMeshProUGUI _introCountdownTimer;
    
    [Header("MiniGame State")]
    [SerializeField] private CanvasGroup _miniGameCanvasGroup;
    [SerializeField] private TextMeshProUGUI _miniGameCountdownTimer;
    
    [Header("Finished State")]
    [SerializeField] private CanvasGroup _finishedCanvasGroup;
    
    [Header("Results State")]
    [SerializeField] private CanvasGroup _resultsCanvasGroup;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private TextMeshProUGUI _rewardText;
    
    //===== State =====
    private UIState _currentState;
    private TextMeshProUGUI _currentCountdown;

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
                    CanvasGroupDisplayer.Show(_finishedCanvasGroup);
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

    private void Awake()
    {
        CurrentState = UIState.Introduction;
        DisableAll();
    }

    public void UpdateCountdown(string time)
    {
        _currentCountdown.text = time;
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
        _introCountdownTimer.text = info.CountdownTimer.ToString();
        _miniGameCountdownTimer.text = info.MiniGameTime.ToString();
    }

    public void OnWinMiniGame(int playerIndex, string reward)
    {
        _playerName.text = GameUtils.PlayerNames[playerIndex];
        _rewardText.text = reward;
        CurrentState = UIState.Results;
    }

    public void DisableAll()
    {
        CanvasGroupDisplayer.Hide(_finishedCanvasGroup);
        CanvasGroupDisplayer.Hide(_introCanvasGroup);
        CanvasGroupDisplayer.Hide(_miniGameCanvasGroup);
        CanvasGroupDisplayer.Hide(_resultsCanvasGroup);
    }

    public enum UIState
    {
        Introduction,
        MiniGame,
        Finished,
        Results
    }
}
