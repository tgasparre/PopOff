using System;
using System.Collections.Generic;
using InputManagement;
using UnityEngine;

public class ActivePlayersTracker : MonoBehaviour
{
    public event Action playerWon;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
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
        //InputManager inputManager = FindAnyObjectByType<InputManager>();

        players.Add(player);
        player.PlayerDied += OnPlayerDied;
        //player.SetInputManager(inputManager);
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
