using UnityEngine;

[CreateAssetMenu(fileName = "Trap", menuName = "Powerups/Trap Powerup")]
public class TrapPowerup : Powerup
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject _trapPrefab;
    [SerializeField] private TrapStats _trapStats;
    
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Trap(_trapPrefab, _trapStats);
    }

    public override void Expire(PlayerPowerups powerups)
    {
        
    }
}

[System.Serializable]
public class TrapStats : PowerupStats
{
   
}
