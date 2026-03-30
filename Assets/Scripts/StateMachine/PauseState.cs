using System;
using UnityEngine;

public class PauseState : GameState
{
    public static event Action<bool> OnPaused;
    public override void EnterState()
    {
        Game.IsFrozen = true;
        OnPaused?.Invoke(true);
        GameCanvas.Instance.ShowPauseScreen();
    }

    public override void ExitState(GameStates newState)
    {
        Game.IsFrozen = false;
        OnPaused?.Invoke(false);
        GameCanvas.Instance.HideAllScreens();
        
        if (newState == GameStates.Menu)
        {
            PlayingState.ExitFromPause();
        }
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Menu;
    }
}
