using UnityEngine;

/// <summary>
/// Handles the minigame introduction, displaying the goal and counting down until start 
/// </summary>
public class MiniGameController : MonoBehaviour
{
    public bool MiniGameStart = false;
    
    public void StartMiniGame()
    {
        //check scene for minigame object
        MiniGameInfo info = FindFirstObjectByType<MiniGameInfo>();
        if (!info)
        {
            Debug.LogError("MiniGameInfo not found! Please add a MiniGameInfo object to the scene!");
            return;
        }

        Game.IsPlayersFrozen = true;
        MiniGameStart = false;
        // GameCanvas.Instance.ShowMiniGameScreen(info, OnFinished);
    }

    private void OnFinished()
    {
        MiniGameStart = true;
        Game.IsPlayersFrozen = false;
    }
}
