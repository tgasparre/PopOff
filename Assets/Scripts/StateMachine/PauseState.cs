using UnityEngine;

public class PauseState : GameState
{
    public override void EnterState()
    {
        Game.IsFrozen = true;
        GameCanvas.Instance.ShowPauseScreen();
    }

    public override void ExitState()
    {
        Game.IsFrozen = false;
        GameCanvas.Instance.HideAllScreens();
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Menu;
    }
}
