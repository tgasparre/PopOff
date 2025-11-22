using System;
using System.Collections.Generic;
using DamageSystem;
using InputManagement;
using UnityEngine;

public class ActivePlayersTracker : MonoBehaviour
{
    public event Action playerWon;
    
    public Transform healthBarContainer;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject healthBarPrefab;
    private int numPlayersAlive;
    private Player winningPlayer;
    private List<Player> players = new List<Player>();

    public void SpawnPlayers(int numPlayers)
    {
        Debug.Log("Spawnplayers called with num players: " + numPlayers);
        for (int i = 0; i < numPlayers; ++i)
        {
            GameObject player = Instantiate(playerPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            RegisterPlayer(player.GetComponent<Player>());
        //    CreatePlayerHealthBar(player.GetComponent<PlayerStats>());
        }
    }

    public string GetWinnerName()
    {
        return winningPlayer.playerStats.PlayerName;
    }

    public void SetNumPlayersAlive(int numPlayers)
    {
        numPlayersAlive = numPlayers;
    }

    public int GetNumPlayersAlive()
    {
        return numPlayersAlive;
    }

    public void BlockPlayerMovement()
    {
        foreach (Player player in players)
        {
            player.FreezePlayerMovement();
        }
    }

    public void UnblockPlayerMovement()
    {
        foreach (Player player in players)
        {
            player.UnfreezePlayerMovement();
        }
    }

    private void OnPlayerDied()
    {
        --numPlayersAlive;
        if (numPlayersAlive <= 1)
        {
            playerWon?.Invoke();
        }
    }

        //subscribes to PlayerDied event so the player will tell us when it drops to 0 hp
        //also adds player to our player list and makes sure they have the correct input manager
    private void RegisterPlayer(Player player)
    {
        players.Add(player);
        player.PlayerDied += OnPlayerDied;
    }

    private void CreatePlayerHealthBar(PlayerStats playerStats)
    {
        GameObject hpBar = Instantiate(healthBarPrefab, healthBarContainer);
        hpBar.GetComponent<HPDisplayer>().Initalize(playerStats);
    }

    //unsubscribe to PlayerDied for each player to prevent memory leaks
    private void OnDestroy()
    { 
        foreach (Player player in players) 
        {
            if (player != null)
            {
                player.PlayerDied -= OnPlayerDied;
            }
        }
    }
}
