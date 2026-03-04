using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    //could use minigame state to 'kill' players in a minigame and still have them respawn in the main game?
    public enum PlayerState
    {
        hitStun,
        regular,
        minigame,
        gameStart
    }
    private PlayerState currentState;

    void Awake()
    {
        currentState = PlayerState.gameStart;
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    public void EnterHitStun()
    {
        currentState = PlayerState.hitStun;
        //disable player movement
    }

    public void EnterPlayerMinigameState()
    {
        currentState = PlayerState.minigame;
    }
    
    public void ResetState()
    {
        currentState = PlayerState.regular;
    }


}

