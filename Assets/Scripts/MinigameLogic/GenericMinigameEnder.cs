using UnityEngine;

public class GenericMinigameEnder : MonoBehaviour
{
    private int numPlayersAlive = Game.Instance.PlayerCount;
    private ActivePlayersTracker activePlayersTracker;

    void Awake()
    {
        activePlayersTracker = Game.Instance.GetActivePlayersTracker();
        if (activePlayersTracker != null)
        {
            Debug.Log("Player Tracker is not null");
            activePlayersTracker.playerDiedInMinigame += OnPlayerDiedInMinigame;
        }
    }

    private void OnPlayerDiedInMinigame()
    {
        Debug.Log("Player died in minigame");
        --numPlayersAlive;
        if (numPlayersAlive <= 1)
        {
            //give this player a power up and transition to the combat state
            PlayingState.CurrentGameplayState = GameplayStates.Combat;
        }
    }

    void OnDestroy()
    {
        if (activePlayersTracker != null)
        { 
            activePlayersTracker.playerDiedInMinigame -= OnPlayerDiedInMinigame;
        }
    }
}
