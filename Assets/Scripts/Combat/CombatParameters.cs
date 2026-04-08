using UnityEngine;

public static class CombatParameters
{
    public const int MAX_PLAYER_HEALTH = 200;
    
    public const int BASIC_ATTACK_DMG = 10;
    public const int ULTIMATE_ATTACK_DMG = 20;
    public const float ATTACK_COOLDOWN = 0.32f;

    public const float KNOCKBACK_DURATION = 0.2f;
    public const float KNOCKBACK_FORCE = 55f; 
    public const float ULTIMATE_KNOCKBACK_FORCE = 75f;
    public static readonly AnimationCurve KNOCKBACK_CURVE = AnimationCurve.EaseInOut(0, 1, 1, 0);
    
    public const float HIT_STUN_DURATION = 0.28f;

    public const float POGO_FORCE = 50;
}

