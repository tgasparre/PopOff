using UnityEngine;
public class CombatState : GameState
{
    public override void EnterState()
    {
        //TODO -- Play little animation
        Loader.LoadCombatScene(StartCountdown);
    }
    
    private void StartCountdown()
    {
        Game.IsPlayersFrozen = true;
        GameCanvas.Instance.StartCombatCountdown(() =>
        {
            Game.IsPlayersFrozen = false;
        });
    }
    
    public override void ExitState(GameStates newState)
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }

    /// <summary>
    /// DEBUG method - starts the combat countdown without loading the combat scene 
    /// </summary>
    public void DEBUG_StartCountdown()
    {
        Game.IsPlayersFrozen = true;
        StartCountdown();
    }
}
