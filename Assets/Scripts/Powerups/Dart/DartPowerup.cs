using UnityEngine;

[CreateAssetMenu(fileName = "Dart", menuName = "Powerups/Dart Powerup")]
public class DartPowerup : Powerup
{
    [Header("Dart Settings")]
    [SerializeField] private GameObject _dartPrefab;
    [SerializeField] private DartStats _dartStats;
    
    public override void UsePowerup(PlayerPowerups powerups)
    {
        powerups.Dart(_dartPrefab, _dartStats);
    }

    public override void Expire(PlayerPowerups powerups)
    {

    }
}

[System.Serializable]
public class DartStats : PowerupStats
{
    [Range(0f, 1.6f)] public float falloffSpeed = 1f;
}
