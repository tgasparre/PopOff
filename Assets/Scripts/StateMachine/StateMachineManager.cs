using UnityEngine;

public class StateMachineManager : MonoBehaviour
{
    public PreStartState PreStartState;
    public PauseState PauseState;
    public GameOverState GameOverState;
    public MiniGameState MiniGameState;
    public PVPCombatState PVPCombatState;
    
    private GameState currentState;

    public void EnterPreStartState()
    {
        SetGameStateTo(PreStartState);
    }

    public void EnterPauseState()
    {
        SetGameStateTo(PauseState);
    }

    public void EnterGameOverState()
    {
        SetGameStateTo(GameOverState);
    }

    public void EnterMiniGameState()
    {
        SetGameStateTo(MiniGameState);
    }

    public void EnterPVPCombatState()
    {
        SetGameStateTo(PVPCombatState);
    }
    
    
    private void SetGameStateTo(GameState newState)
    {
        if (currentState != null)
        {
            currentState.ExitState();
        }
        currentState = newState;
        Debug.Log(currentState);
        
        currentState.EnterState();
    }
}
