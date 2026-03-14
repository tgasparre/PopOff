using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private CanvasGroup _controlsMenu;
    [SerializeField] private CanvasGroup _creditsMenu;
    
    public void OnStartClicked()
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            Game.CanJoin = true;
        });
    }

    public void OnControlsClicked(Button selectionHandler)
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            selectionHandler.Select();
            CanvasGroupDisplayer.Show(_controlsMenu);
        });
    }

    public void OnCreditsClicked(Button selectionHandler)
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            selectionHandler.Select();
            CanvasGroupDisplayer.Show(_creditsMenu);
        });
    }

    public void OnQuitGameClicked()
    {
        GameCanvas.Instance.Transition(completed: Game.ExitGame);
    }

    public void ReturnToMainMenu(Button selectionHandler)
    {
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            selectionHandler.Select();
            CanvasGroupDisplayer.Show(_mainMenu);
        });
    }

    private void DisableAll()
    {
         CanvasGroupDisplayer.Hide(_mainMenu);
         CanvasGroupDisplayer.Hide(_controlsMenu);
         CanvasGroupDisplayer.Hide(_creditsMenu);
    }
}