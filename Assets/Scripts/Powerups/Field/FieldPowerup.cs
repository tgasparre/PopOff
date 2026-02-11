using UnityEngine;

[CreateAssetMenu(fileName = "Field", menuName = "Powerups/Field Powerup")]
public class FieldPowerup : Powerup
{
    [Header("Field Settings")]
    [SerializeField] private FieldStats _stats; 
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Field(_stats);
    }

    public override void Expire(PlayerPowerups powerups)
    {
        powerups.DisableField();
    }
}

[System.Serializable]
public struct FieldStats
{
    [SerializeField] private float size;
    public float Size => (size == 0) ? 1f : size;
    [SerializeField] private float force;
    public float Force => (force == 0) ? 1f : force;
    public bool isPullTowards;
}
