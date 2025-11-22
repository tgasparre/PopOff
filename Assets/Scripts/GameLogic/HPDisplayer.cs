using System;
using UnityEngine;

public class HPDisplayer : MonoBehaviour
{
    public RectTransform hpBar;
    private float fullSize;
    private PlayerStats playerStats;

    private void Awake()
    {
        fullSize = hpBar.sizeDelta.x;
        
    }

    public void Initalize(PlayerStats connectedPlayer)
    {
        playerStats = connectedPlayer;
    }

    void Update()
    {
        if (playerStats == null) return;

        float pct = playerStats.HP / 200;

        // Update width
        hpBar.sizeDelta = new Vector2(fullSize * pct, hpBar.sizeDelta.y);
    }
    
}
