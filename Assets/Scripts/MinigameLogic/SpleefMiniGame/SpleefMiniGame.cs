using System;
using System.Collections;
using UnityEngine;

public class SpleefMiniGame : MiniGameInfo
{
    [Space]
    [SerializeField] private GameObject _startingPlatformParent;
    
    protected override void StartMiniGame()
    {
        _startingPlatformParent.SetActive(false);
    }
}
