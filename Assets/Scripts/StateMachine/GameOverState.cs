
public class GameOverState : GameState
{

    public override void EnterState()
    {
        // delete objects on stage
        // stop any player movement
        // go to end screen
        uiHandler.SwitchToEndScreen();
    }
}
