using UnityEngine;

public static class CombatParameters
{
    public static int basicAttackDamage = 20;
    public static int secondaryAttackDamage = 40;
    
    public static float knockbackDuration = 0.2f;
    public static float knockbackForce = 12f;
    public static AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    
    public static float ultimateKnockbackForce = 14f;
    
    
    public static float hitStunDuration = 0.2f;
}

