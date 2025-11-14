using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    
    public int numPlayersAlive;
    [SerializeField] private StateMachineManager gameStateManager;
    public GameTools gameTools = new GameTools();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        ShowStartMenus();
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    public void Reset()
    {
        // go back to title screen or character select screen
        // stop all movement/functions/coroutines happening
        gameStateManager.EnterPreStartState();
    }

    public void StartGame()
    {
        // move to player number select, then start minigame
        // maybe add another method here to proc player select, then in that method move to minigame state
        //spawn in players into main fighting arena
        gameStateManager.EnterPVPCombatState();
    }

    public void PauseGame()
    {
        gameStateManager.EnterPauseState();
    }

    public void ResumeGame()
    {
        //will have to change this to be able to pause from minigame state
        gameStateManager.EnterPVPCombatState();
    }

    public void EndGame()
    {
        //TODO: edit this to display name of which player won
        gameStateManager.EnterGameOverState();
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
        gameStateManager.EnterMiniGameState();
    }

    public void EndMinigame()
    {
        gameStateManager.EnterPVPCombatState();
    }

    public void SetNumPlayersAlive(int numPlayers)
    {
        numPlayersAlive = numPlayers;
    }

    private void KillPlayer()
    {
        numPlayersAlive--;
        if (numPlayersAlive <= 0)
        {
            EndGame();
        }
    }

    private void ShowStartMenus()
    {
        if (!gameTools.HasGameStarted())
        {
            gameStateManager.EnterPreStartState();
            gameTools.SetGameStarted(true);
        }
    }
    
}
