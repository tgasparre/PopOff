using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public CanvasGroup StartScreen;
    public CanvasGroup EndScreen;
    public CanvasGroup PauseScreen;


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
    
}
