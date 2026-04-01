public class MenuState : GameState
{
    public override void EnterState()
    {
       Loader.LoadMenuScene();
       ActivePlayerTracker.DestroyPlayers();
       Game.CanJoin = false;
    }

    public override void ExitState(GameStates newState)
    {
        Game.CanJoin = false;
    }

    public override bool IsStateSwitchable(GameStates test)
    {
        return test is GameStates.Playing or GameStates.Pause;
    }
}
