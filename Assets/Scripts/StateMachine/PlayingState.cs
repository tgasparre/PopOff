using System;
using UnityEngine;

public class PlayingState : GameState
{
    public static readonly CombatState combatState = new CombatState();
    public static readonly MiniGameState miniGameState = new MiniGameState();

    public static bool IsStarting { get; private set; } = true;  
    private static GameState _activeState = miniGameState;
    private static GameplayStates _currentState = GameplayStates.MiniGame;
    public static GameplayStates CurrentGameplayState
    {
        get => _currentState;
        set
        {
            switch (value)
            {
                case GameplayStates.Combat:
                    SetState(combatState, GameStates.Playing);
                    break;
                case GameplayStates.MiniGame:
                    SetState(miniGameState, GameStates.Playing);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            _currentState = value;
            return;

            static void SetState(GameState state, GameStates type)
            {
                if (state != _activeState) _activeState?.ExitState(type);

                _activeState = state;
                _activeState.EnterState();
            } 
        }
    }

    public override void EnterState()
    {
        if (Game.currentState == GameStates.Menu)
        {
            IsStarting = true;
            CurrentGameplayState = GameplayStates.MiniGame;
        }

        IsStarting = false;
    }

    public override void ExitState(GameStates newState)
    {

    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Pause or GameStates.GameOver or GameStates.Playing;
    }

    public static void ExitFromPause()
    {
        _activeState.ExitState(GameStates.Menu);
    }

    /// <summary>
    /// DEBUG method -- sets the state of the game without calling the ExitState or EnterState methods.
    /// </summary>
    /// <param name="state"></param>
    public static void DEBUG_SetGamePlayState(GameplayStates state)
    {
        _activeState = state switch
        {
            GameplayStates.Combat => combatState,
            GameplayStates.MiniGame => miniGameState,
            _ => _activeState
        };
        _currentState = state;
        IsStarting = false;
    }

    /// <summary>
    /// DEBUG method -- starts the minigame, provided there is one present in the scene
    /// </summary>
    public static void DEBUG_StartMiniGame()
    {
        miniGameState.StartMiniGame();
    }

    /// <summary>
    /// DEBUG method -- start the combat countdown when entering combat scenes
    /// </summary>
    public static void DEBUG_StartCombatCountdown()
    {
        combatState.DEBUG_StartCountdown();
    }
}

public enum GameplayStates
{
    Combat,
    MiniGame,
}