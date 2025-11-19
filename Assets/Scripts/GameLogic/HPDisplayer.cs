using UnityEngine;

public class HPDisplayer : MonoBehaviour
{
    public PlayerStats playerStats;
    public RectTransform hpBar;

    void Update()
    {
        hpBar.sizeDelta = new Vector2(playerStats.HP, hpBar.sizeDelta.y);
    }
    
}
