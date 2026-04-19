using UnityEngine;

public class MenuState : GameState
{
    public override void EnterState()
    {
        Loader.LoadMenuScene();
       ActivePlayerTracker.DestroyPlayers();
       Game.CanJoin = Game.FreezeJoin = false;
       AudioManager.SwitchMusic(MusicType.Menu);
    }

    public override void ExitState(GameStates newState)
    {
        if (newState == GameStates.Pause)
        {
            Game.FreezeJoin = true;
        }
        else Game.CanJoin = false;
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Pause;
    }
}
