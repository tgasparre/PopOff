using System;
using UnityEngine;

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
    [SerializeField] private GameOverController _gameOverScreen;
    private TransitionController _transitionController;

    public void Unpause()
    {
        Game.currentState = GameStates.Playing;
    }

    public void ReturnToMenu()
    {
        Game.currentState = GameStates.Menu;
    }

    public void Transition(TransitionType transitionType = TransitionType.Menu, Action completed = null)
    {
        _transitionController.Transition(transitionType, completed);
    }

    public void ShowPauseScreen()
    {
        HideAllScreens();
        CanvasGroupDisplayer.Show(_pauseScreen);
    }

    public void ShowGameOverScreen()
    {
        HideAllScreens();
        _gameOverScreen.SetWinnerName();
        CanvasGroupDisplayer.Show(_gameOverScreen.canvasGroup);
    }

    public void HideAllScreens()
    {
        CanvasGroupDisplayer.Hide(_pauseScreen);
        CanvasGroupDisplayer.Hide(_gameOverScreen.canvasGroup);
    }
}
