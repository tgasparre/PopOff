using UnityEngine;
public class CombatState : GameState
{
    public override void EnterState()
    {
        Time.timeScale = 0f;
        //TODO -- Play little animation
        Loader.LoadCombatScene();
        
        // //unfreeze all player movement after minigame
        // Game.Instance.GetActivePlayersTracker().UnfreezeAllPlayers();
    }
    
    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
}
