using System;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public CanvasGroup StartScreen;
    public CanvasGroup EndScreen;
    public CanvasGroup PauseScreen;
    public CanvasGroup InfoScreen;

    private void Awake()
    {
        CanvasGroupDisplayer.Hide(InfoScreen);
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

    public void SwitchToPvPState()
    {
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
        CanvasGroupDisplayer.Hide(StartScreen);
        CanvasGroupDisplayer.Hide(PauseScreen);
        CanvasGroupDisplayer.Hide(EndScreen);
    }
    
    
}
