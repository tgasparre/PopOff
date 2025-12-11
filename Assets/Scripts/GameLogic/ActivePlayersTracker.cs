using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivePlayersTracker : MonoBehaviour
{
    public event Action playerWon;
    
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private Transform uiLayoutGroup;
    
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject playerPrefab;
    private int numPlayersAlive;
    private Player winningPlayer;
    private List<Player> players = new List<Player>();
    
    [SerializeField] private PlayerInputManager inputManager;

    private void Awake() {
        // Subscribe to the player joined event
        inputManager.onPlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(PlayerInput playerInput) 
	{
        Debug.Log("OnPlayerJoined was called");
        
        Player playerScript = playerInput.GetComponent<Player>();
        RegisterPlayer(playerScript);
        
        // Spawn UI
        GameObject uiInstance = Instantiate(playerUIPrefab, uiLayoutGroup);
        PlayerUIDisplayer playerUIDisplayer = uiInstance.GetComponent<PlayerUIDisplayer>();
        
        // Connect hurtbox to UI
        playerUIDisplayer.InitializePlayerUI(playerInput.GetComponentInChildren<AttackHurtbox>());
        
        // Connect tracker to player
        UltimateAttackTracker tracker = playerInput.GetComponent<UltimateAttackTracker>();
        
        if (tracker != null && playerUIDisplayer != null)
        {
			//connect tracker to UI 
            tracker.SetPlayerUI(playerUIDisplayer);
            //let the UI know when the ultimate attack is unlocked
            playerUIDisplayer.SubscribeToTracker(tracker);
            Debug.Log("Successfully connected tracker to UI");
        }
        else
        {
            Debug.LogError("Failed to find tracker or UI displayer");
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

    public void ResetAllPlayerTrackers()
    {
        foreach (Player player in players)
        {
            player.gameObject.GetComponent<UltimateAttackTracker>().ResetTracker();
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
        
        if (inputManager != null) {
            inputManager.onPlayerJoined -= OnPlayerJoined;
        }
    }
}
