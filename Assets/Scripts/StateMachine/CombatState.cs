using UnityEngine;
public class CombatState : GameState
{
    public override void EnterState()
    {
        Debug.Log("enter combat state");
        Time.timeScale = 0f;
        //TODO -- Play little animation
        // Game.Instance.LoadCombat();

        Loader.LoadCombatScene();

        // if (sceneLoader.IsInMinigameScene())
        // {
        //     Debug.Log("game detected in mini game scene");
        //     // hardcoded for now, will change later to support different combat stages/arenas
        //     sceneLoader.InstantLoadScene("SampleScene");
        // }
        // // minigamePortalPlacer.StartPlacing();
        //  uiHandler.SwitchToPlayingState();
    }
    
    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        throw new System.NotSupportedException();
    }
}
