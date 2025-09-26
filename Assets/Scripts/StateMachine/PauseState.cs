
public class PauseState : GameState
{
    public override void EnterState()
    {
        //freeze movement
        //change screen/panel to pause panel 
        uiHandler.SwitchToPauseScreen();
    }
}
