using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    // possibly switch these into a state machine handler class if it gets too messy
    public PreStartState PreStartState;
    public PauseState PauseState;
    public GameOverState GameOverState;
    public MiniGameState MiniGameState;
    public PVPCombatState PVPCombatState;

    public int numPlayersAlive;


    private GameState currentState;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetGameStateTo(PreStartState);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            TriggerMinigame();
        }
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
        // maybe add another method here to proc player select, then in that method move to minigame state
        //spawn in players into main fighting arena
        SetGameStateTo(PVPCombatState);
    }

    public void PauseGame()
    {
        SetGameStateTo(PauseState);
    }

    public void ResumeGame()
    {
        SetGameStateTo(PVPCombatState);
    }

    public void EndGame()
    {
        SetGameStateTo(GameOverState);
    }

    public void CloseGame()
    { 
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
    }

    public void TriggerMinigame()
    {
        SetGameStateTo(MiniGameState);
    }

    public void EndMinigame()
    {
        SetGameStateTo(PVPCombatState);
    }

    private void SetGameStateTo(GameState newState)
    {
        // if pvpcombatstate, stop spawning portals, else does nothing
        currentState = newState;
        Debug.Log(currentState);
        currentState.EnterState();
    }

    
    
}
