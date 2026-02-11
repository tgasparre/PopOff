using UnityEngine;

[CreateAssetMenu(fileName = "Dart", menuName = "Powerups/Dart Powerup")]
public class DartPowerup : Powerup
{
    [Header("Dart Settings")]
    [SerializeField] private GameObject _dartPrefab;
    [SerializeField] private DartStats _stats;
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Dart(_dartPrefab, _stats);
    }

    public override void Expire(PlayerPowerups powerups)
    {

    }
}

[System.Serializable]
public struct DartStats
{
    public float speed;
    [SerializeField] private float size;
    public float Size => (size == 0) ? 1f : size;
    
    [SerializeField] [Range(0f, 1.6f)] private float falloffSpeed;
    public float FalloffSpeed => (falloffSpeed == 0) ? 1f : falloffSpeed;
    
    public float extraDamage;
}
