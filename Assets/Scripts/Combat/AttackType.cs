using UnityEngine;

[CreateAssetMenu(menuName = "AttackType", fileName = "New Attack")]
public class AttackType : ScriptableObject
{
    public int Damage;
    public Vector2 Knockback;
    public float StunDuration;
    public float CooldownDuration;
}
