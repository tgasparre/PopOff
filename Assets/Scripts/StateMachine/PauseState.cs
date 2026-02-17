using UnityEngine;

public class PauseState : GameState
{
    public override void EnterState()
    {
        Time.timeScale = 0f;
        GameCanvas.Instance.ShowPauseScreen();
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
