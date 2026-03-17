using System;
using System.Collections;
using UnityEngine;

public class CoinMiniGame : MiniGameInfo
{
    [Space]
    [SerializeField] private CoinMinigameEnder _coin;
    
    protected override void StartMiniGame()
    {
        _coin.OnCollected += TriggerEndMiniGame;
    }
}
