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

    public override void Expire()
    {
        // throw new System.NotImplementedException();
    }
}

[System.Serializable]
public struct DartStats
{
    public float speed;
    public float size;
    public float falloffSpeed;
    public float damage;
    public bool doesExplode;
}
