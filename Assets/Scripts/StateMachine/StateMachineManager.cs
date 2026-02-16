using System;
using UnityEngine;

public static class StateMachineManager
{
    public static readonly MenuState menuState = new MenuState();
    public static readonly PauseState pauseState = new PauseState();
    public static readonly PlayingState playingState = new PlayingState();
    public static readonly GameOverState gameOverState = new GameOverState();

    public static GameStates currentState { private set; get; }
    private static GameState _activeState = menuState;
    
    public static void SwitchState(GameStates state)
    {
        if (!_activeState.IsStateSwitchable(state))
        {
            Debug.LogWarning("State could not switch: " + _activeState + " and " + state);
            return;
        }
        
        switch (state)
        {
            case GameStates.Menu:
                SetGameStateTo(menuState);
                break;
            case GameStates.Pause:
                SetGameStateTo(pauseState);
                break;
            case GameStates.Playing:
                SetGameStateTo(playingState);
                break;
            case GameStates.GameOver:
                SetGameStateTo(gameOverState);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        currentState = state;
    }

    private static void SetGameStateTo(GameState state)
    {
        if (state != _activeState) _activeState?.ExitState();
        
        _activeState = state;
        _activeState.EnterState();
    }
    
    /// <summary>
    /// DEBUG method -- sets the state of the game without calling the ExitState or EnterState methods.
    /// </summary>
    /// <param name="state"></param>
    public static void DEBUG_SetGameState(GameStates state)
    {
        _activeState = state switch
        {
            GameStates.Menu => menuState,
            GameStates.Playing => playingState,
            GameStates.Pause => pauseState,
            GameStates.GameOver => gameOverState,
            _ => _activeState
        };
        currentState = state;
    }
}

public enum GameStates
{
    Menu,
    Pause, 
    Playing,
    GameOver
}
