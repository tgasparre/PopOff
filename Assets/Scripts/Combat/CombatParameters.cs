using UnityEngine;

public static class CombatParameters
{
    public static int basicAttackDamage = 20;
    public static int secondaryAttackDamage = 40;
    
    public static float knockbackDuration = 0.2f;
    public static float knockbackForce = 10f;
    public static AnimationCurve knockbackCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
}

