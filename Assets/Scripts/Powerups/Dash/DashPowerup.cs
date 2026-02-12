using UnityEngine;

[CreateAssetMenu(fileName = "Dash", menuName = "Powerups/Dash Powerup", order = 0)]
public class DashPowerup : Powerup
{
    [Header("Dash Settings")]
    [SerializeField] private DashStats _dashStats ;
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Dash(_dashStats);
    }

    public override void Expire(PlayerPowerups powerups)
    {
        // throw new System.NotImplementedException();
    }
}

[System.Serializable]
public struct DashStats
{
    public float dashForce;
    public float yForce;
}
