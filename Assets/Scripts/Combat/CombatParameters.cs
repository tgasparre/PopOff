using UnityEngine;

public static class CombatParameters
{
    public const int basicAttackDamage = 20;
    public const int ultimateAttackDamage = 40;

    public const float knockbackDuration = 0.2f;
    public const float knockbackForce = 3.5f;
    public static readonly AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    public const float ultimateKnockbackForce = 5f;
    public const float hitStunDuration = 0.2f;
}

