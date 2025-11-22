
public class PauseState : GameState
{
    public ActivePlayersTracker activePlayersTracker;
    public override void EnterState()
    {
        activePlayersTracker.BlockPlayerMovement();
        uiHandler.SwitchToPauseScreen();
    }

    public override void ExitState()
    {
        activePlayersTracker.UnblockPlayerMovement();
    }
}
