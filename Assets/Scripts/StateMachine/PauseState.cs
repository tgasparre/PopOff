using UnityEngine;

public class PauseState : GameState
{
    public override void EnterState()
    {
        Time.timeScale = 0f;
        CanvasGroupDisplayer.Show(GameCanvas.Instance.PauseScreen);
    }

    public override void ExitState()
    {
        Time.timeScale = 1f;
        CanvasGroupDisplayer.Hide(GameCanvas.Instance.PauseScreen);
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Menu;
    }
}
