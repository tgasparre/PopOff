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
        
        CanvasGroupDisplayer.Hide(_pauseScreen);
    }

    [SerializeField] private CanvasGroup _pauseScreen;
    public CanvasGroup PauseScreen => _pauseScreen;
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
}
