using UnityEngine;
public class MenuState : GameState
{
    public override void EnterState()
    {
       Loader.LoadMenuScene();
       Game.Instance.DestroyPlayers();
       Game.Instance.CanJoin = false;
    }

    public override void ExitState()
    {
        
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test == GameStates.Playing;
    }
}
