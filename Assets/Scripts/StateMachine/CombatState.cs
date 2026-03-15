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
        GameCanvas.Instance.StartCombatCountdown(FinishCountdown);
    }

    private void FinishCountdown()
    {
        Game.IsPlayersFrozen = false;
    }
    
    
    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }

    public void DEBUG_StartCountdown()
    {
        Game.IsPlayersFrozen = true;
        StartCountdown();
    }
}
