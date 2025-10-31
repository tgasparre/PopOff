using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class PVPCombatState : GameState
{
    [SerializeField]
    private MiniGamePortalPlacer minigamePortalPlacer;

    public UnityEvent SceneLoaded;
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
        SceneLoaded?.Invoke();
    }
    
    public override void EndState()
    {
        minigamePortalPlacer.StopPlacing();
    }
    
}
