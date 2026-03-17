using System.Collections.Generic;
using UnityEngine;

public class GenericMinigameEnder : MonoBehaviour
{
    [SerializeField] private Powerup[] powerups;
    private int numPlayersAlive = Game.PlayerCount;
    private ActivePlayersTracker activePlayersTracker;

    void Awake()
    {
        //activePlayersTracker = Game.Instance.ActivePlayerTracker;
        if (activePlayersTracker != null)
        {
            //activePlayersTracker.playerWonMinigame += OnPlayerWonMinigame;
        }
    }

    private void OnPlayerWonMinigame(Player player)
    {
        PlayingState.CurrentGameplayState = GameplayStates.Combat;
        player.powerups.ApplyPower(GetRandomPowerup());
    }

    private Powerup GetRandomPowerup()
    {
        return powerups[Random.Range(0, powerups.Length)];
    }

    void OnDestroy()
    {
        if (activePlayersTracker != null)
        { 
            //activePlayersTracker.playerWonMinigame -= OnPlayerWonMinigame;
        }
    }
}
