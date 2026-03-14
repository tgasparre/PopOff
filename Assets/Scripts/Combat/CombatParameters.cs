using UnityEngine;

public static class CombatParameters
{
    public static int basicAttackDamage = 20;
    public static int secondaryAttackDamage = 40;
    
    public static readonly float knockbackDuration = 0.2f;
    public static readonly float knockbackForce = 3.5f;
    public static readonly AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    
    public static readonly float ultimateKnockbackForce = 5f;
    
    
    public static readonly float hitStunDuration = 0.2f;
}

