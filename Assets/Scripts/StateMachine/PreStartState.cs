
public class PreStartState : GameState
{
    public override void EnterState()
    {
        // if any processes are going on in the game, end them
        uiHandler.SwitchToStartScreen();
    }
    
}
