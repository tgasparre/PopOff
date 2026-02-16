using System;
using System.Collections;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private CanvasGroup _playerSelectionMenu;
    [SerializeField] private CanvasGroup _controlsMenu;
    [SerializeField] private CanvasGroup _creditsMenu;
    
    public void OnStartClicked()
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            Game.Instance.CanJoin = true;
        });
    }

    public void OnControlsClicked()
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            CanvasGroupDisplayer.Show(_controlsMenu);
        });
    }

    public void OnCreditsClicked()
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            CanvasGroupDisplayer.Show(_creditsMenu);
        });
    }

    public void OnQuitGameClicked()
    {
        GameCanvas.Instance.Transition(completed: Game.ExitGame);
    }

    public void ReturnToMainMenu()
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            CanvasGroupDisplayer.Show(_mainMenu);
        });
    }

    private void DisableAll()
    {
         CanvasGroupDisplayer.Hide(_mainMenu);
         CanvasGroupDisplayer.Hide(_controlsMenu);
         CanvasGroupDisplayer.Hide(_creditsMenu);
         CanvasGroupDisplayer.Hide(_playerSelectionMenu);
    }
}