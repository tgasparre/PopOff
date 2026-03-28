using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _mainMenu;
    [SerializeField] private CanvasGroup _controlsMenu;
    [SerializeField] private CanvasGroup _creditsMenu;

    [SerializeField] private GameObject _joinText;

    private void Awake()
    {
        _joinText.SetActive(false);
    }

    public void OnStartClicked()
    {
        AudioManager.PlaySound(AudioTrack.ButtonClick);
        
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            Game.IsPlayersFrozen = false;
            Game.CanJoin = true;
            
            _joinText.SetActive(true);
        });
    }

    public void OnControlsClicked(Button selectionHandler)
    {
        AudioManager.PlaySound(AudioTrack.ButtonClick);
        
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            selectionHandler.Select();
            CanvasGroupDisplayer.Show(_controlsMenu);
        });
    }

    public void OnCreditsClicked(Button selectionHandler)
    {
        AudioManager.PlaySound(AudioTrack.ButtonClick);
        
        GameCanvas.Instance.Transition(completed: () =>
        {
            DisableAll();
            selectionHandler.Select();
            CanvasGroupDisplayer.Show(_creditsMenu);
        });
    }

    public void OnQuitGameClicked()
    {
        AudioManager.PlaySound(AudioTrack.ButtonClick);
        
        GameCanvas.Instance.Transition(completed: Game.ExitGame);
    }

    public void ReturnToMainMenu(Button selectionHandler)
    {
        AudioManager.PlaySound(AudioTrack.ButtonClick);
        
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