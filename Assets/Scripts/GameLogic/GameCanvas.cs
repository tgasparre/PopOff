using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameCanvas : MonoBehaviour
{
    public static GameCanvas Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _transitionController = GetComponentInChildren<TransitionController>();
    }

    private void Start()
    {
        HideAllScreens();
    }

    [SerializeField] private CanvasGroup _pauseScreen;
    [SerializeField] private MiniGameUI _miniGameScreen;
    [SerializeField] private CombatUI _combatScreen;
    [SerializeField] private GameOverController _gameOverScreen;
    [SerializeField] private PlayerUIController _playerUIController;
    private TransitionController _transitionController;
    
    [Space]
    [SerializeField] private Button _unpauseButton;
    [SerializeField] private Button _playAgainButton;

    public static IMiniGameUI MiniGameUI => Instance._miniGameScreen;

    public void Unpause()
    {
        Game.currentState = GameStates.Playing;
    }

    public void ReturnToMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Game.currentState = GameStates.Menu;
    }

    public void Transition(TransitionType transitionType = TransitionType.Menu, Action completed = null)
    {
        _transitionController.Transition(transitionType, completed);
    }

    public void ShowPauseScreen()
    {
        HideAllScreens();
        _unpauseButton.Select();
        CanvasGroupDisplayer.Show(_pauseScreen);
    }

    public void ShowGameOverScreen()
    {
        HideAllScreens();
        _playAgainButton.Select();
        _gameOverScreen.SetWinnerName();
        CanvasGroupDisplayer.Show(_gameOverScreen.canvasGroup);
    }

    public void OnWinMiniGame(int playerIndex, string reward)
    {
        _miniGameScreen.OnWinMiniGame(playerIndex, reward);
    }

    public void HideMiniGameDescription()
    {
        _miniGameScreen.HideDescription();
    }

    public void StartCombatCountdown(Action onFinished)
    {
        CanvasGroupDisplayer.Show(_combatScreen.Group);
        _combatScreen.StartCombatCountdown(onFinished);
    }

    public void CreatePlayerUI(PlayerController playerController)
    {
        _playerUIController.CreatePlayerUI(playerController);
    }

    public void DestroyUI()
    {
        _playerUIController.DestroyUI();
    }

    public void HideAllScreens()
    {
        CanvasGroupDisplayer.Hide(_pauseScreen);
        CanvasGroupDisplayer.Hide(_combatScreen.Group);
        CanvasGroupDisplayer.Hide(_gameOverScreen.canvasGroup);
    }
}
