using UnityEngine;

[CreateAssetMenu(fileName = "Trap", menuName = "Powerups/Trap Powerup")]
public class TrapPowerup : Powerup
{
    [Header("Trap Settings")]
    [SerializeField] private GameObject _trapPrefab;
    [SerializeField] private float _lifetime;
    [SerializeField] private int _damage;
    
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Trap(_trapPrefab, _lifetime, _damage);
    }

    public override void Expire(PlayerPowerups powerups)
    {
        
    }
}
