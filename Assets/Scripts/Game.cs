using UnityEngine;

public class Game : MonoBehaviour
{
    // possibly switch these into a state machine handler class if it gets too messy
    public PreStartState PreStartState;
    public PauseState PauseState;
    public GameOverState GameOverState;


    private GameState currentState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameStateTo(PreStartState);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        // go back to title screen or character select screen
        // stop all movement/functions/coroutines happening
        SetGameStateTo(PreStartState);
    }

    public void StartGame()
    {
        // move to player number select, then start minigame
    }

    public void PauseGame()
    {
        SetGameStateTo(PauseState);
    }

    private void SetGameStateTo(GameState newState)
    {
        currentState = newState;
        Debug.Log(currentState);
        currentState.EnterState();
    }
}
