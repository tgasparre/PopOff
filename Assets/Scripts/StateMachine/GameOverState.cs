
using UnityEngine;

public class GameOverState : GameState
{
    public override void EnterState()
    {
        // delete objects on stage
        // stop any player movement
        // go to end screen
        // uiHandler.SwitchToEndScreen();
        
        //TODO -- play animation of winner
        Time.timeScale = 0f;
        GameCanvas.Instance.ShowGameOverScreen();
       // GameCanvas.Instance.GameOverController.SetWinnerName("s");
    }

    public override void ExitState()
    {
        Time.timeScale = 1f;
        GameCanvas.Instance.HideAllScreens();
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Menu;
    }
}
