using System.Collections.Generic;
using UnityEngine;

public class GenericMinigameEnder : MonoBehaviour
{
    [SerializeField] private Powerup[] powerups;
    private int numPlayersAlive = Game.Instance.PlayerCount;
    private ActivePlayersTracker activePlayersTracker;

    void Awake()
    {
        activePlayersTracker = Game.Instance.GetActivePlayersTracker();
        if (activePlayersTracker != null)
        {
            activePlayersTracker.playerDiedInMinigame += OnPlayerDiedInMinigame;
        }
    }

    private void OnPlayerDiedInMinigame(Player player)
    {
        Debug.Log("Player died in minigame");
        --numPlayersAlive;
        if (numPlayersAlive <= 1)
        {
            PlayingState.CurrentGameplayState = GameplayStates.Combat;
            player.powerups.ApplyPower(GetRandomPowerup());
        }
    }

    private Powerup GetRandomPowerup()
    {
        return powerups[Random.Range(0, powerups.Length)];
    }

    void OnDestroy()
    {
        if (activePlayersTracker != null)
        { 
            activePlayersTracker.playerDiedInMinigame -= OnPlayerDiedInMinigame;
        }
    }
}
