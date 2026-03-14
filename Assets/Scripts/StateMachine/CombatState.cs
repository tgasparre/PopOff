using UnityEngine;
public class CombatState : GameState
{
    public override void EnterState()
    {
        Game.IsPlayersFrozen = true;
        //TODO -- Play little animation
        Loader.LoadCombatScene(StartCountdown);
        return;

        void StartCountdown()
        {
            GameCanvas.Instance.StartCombatCountdown(FinishCountdown);
        }

        void FinishCountdown()
        {
            Game.IsPlayersFrozen = false;
        }
    }
    
    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
}
