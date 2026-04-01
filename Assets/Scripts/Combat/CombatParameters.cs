using UnityEngine;

public static class CombatParameters
{
    public const int basicAttackDamage = 10;
    public const int ultimateAttackDamage = 20;

    public const float knockbackDuration = 0.2f;
    public const float knockbackForce = 15f; 
    public static readonly AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    public const float ultimateKnockbackForce = 18f;
    public const float hitStunDuration = 0.5f;

    public const float MAX_PLAYER_HEALTH = 200f;
}

