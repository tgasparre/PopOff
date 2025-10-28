using System.Collections;
using UnityEngine;

public class PVPCombatState : GameState
{
    [SerializeField]
    private MiniGamePortalPlacer minigamePortalPlacer;
    public override void EnterState()
    {
        if (sceneLoader.IsInMinigameScene())
        {
            Debug.Log("game detected in mini game scene");
            // hardcoded for now, will change later to support different combat stages/arenas
            sceneLoader.InstantLoadScene("SampleScene");
        }
        // minigamePortalPlacer.StartPlacing();
        uiHandler.SwitchToPlayingState();
    }

    public override void EndState()
    {
        minigamePortalPlacer.StopPlacing();
    }
    
    
    
    
}
