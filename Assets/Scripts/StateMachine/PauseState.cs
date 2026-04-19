using System;
using UnityEngine;

public class PauseState : GameState
{
    public static event Action<bool> OnPaused;
    public static GameStates RestoreState { get; private set; }
    
    public override void EnterState()
    {
        Game.IsFrozen = true;
        OnPaused?.Invoke(true);
        GameCanvas.Instance.ShowPauseScreen();

        RestoreState = StateMachineManager.CurrentState;
    }

    public override void ExitState(GameStates newState)
    {
        Game.IsFrozen = false;
        Game.FreezeJoin = false;
        OnPaused?.Invoke(false);
        GameCanvas.Instance.HideAllScreens();
        
        if (newState == GameStates.Menu)
        {
            PlayingState.ExitFromPause();

            Game.CanJoin = false;
            Game.currentState = GameStates.Menu;
            StateMachineManager.menuState.EnterState();
        }
        else Game.currentState = RestoreState;
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Menu;
    }
}
