using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    public CanvasGroup StartScreen;
    public CanvasGroup EndScreen;
    public CanvasGroup PauseScreen;
    public CanvasGroup InfoScreen;
    public CanvasGroup PlayerSelectScreen;
    public CanvasGroup ControlsScreen;
    public static UIHandler Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        CanvasGroupDisplayer.Hide(InfoScreen);
        CanvasGroupDisplayer.Hide(PlayerSelectScreen);
        CanvasGroupDisplayer.Hide(ControlsScreen);
    }
    

    public void SwitchToStartScreen()
    {
        CanvasGroupDisplayer.Hide(EndScreen);
        CanvasGroupDisplayer.Hide(PauseScreen);
        
        CanvasGroupDisplayer.Show(StartScreen);
    }

    public void SwitchToEndScreen()
    {
        CanvasGroupDisplayer.Hide(StartScreen);
        CanvasGroupDisplayer.Hide(PauseScreen);
        
        CanvasGroupDisplayer.Show(EndScreen);
    }


    public void SwitchToPauseScreen()
    {
        CanvasGroupDisplayer.Hide(StartScreen);
        CanvasGroupDisplayer.Hide(EndScreen);
        
        CanvasGroupDisplayer.Show(PauseScreen);
    }

    public void SwitchToPlayingState()
    {
        Debug.Log("Switched to playing state");
        ClearAllMenus();
    }

    public void DisplayPanel(CanvasGroup panel)
    {
        CanvasGroupDisplayer.Show(panel);
    }

    public void HidePanel(CanvasGroup panel)
    {
        CanvasGroupDisplayer.Hide(panel);
    }

    private void ClearAllMenus()
    {
        // yield return null;
        CanvasGroupDisplayer.Hide(StartScreen);
        CanvasGroupDisplayer.Hide(PauseScreen);
        CanvasGroupDisplayer.Hide(EndScreen);
        CanvasGroupDisplayer.Hide(InfoScreen);
        CanvasGroupDisplayer.Hide(PlayerSelectScreen);
        CanvasGroupDisplayer.Hide(ControlsScreen);
        Debug.Log("clear all menus was called in " + SceneManager.GetActiveScene().name);
    }
    
    
}
