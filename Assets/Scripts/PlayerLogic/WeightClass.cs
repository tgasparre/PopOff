using System;
using UnityEngine;
[System.Serializable]
public struct WeightClass
{
    public WeightClassType type;
    public float knockbackMultiplier;
    public float damageMultiplier;

    public WeightClass(WeightClassType weightClass, float knockback, float damage)
    {
        type = weightClass;
        knockbackMultiplier = knockback;
        damageMultiplier = damage;
    }

    public void ChangeWeightClass(WeightClassType weightClass)
    {
        type = weightClass;
        switch (type)
        {
            case WeightClassType.Default:
                knockbackMultiplier = WeightParameters.regularKnockbackMultiplier;
                damageMultiplier = WeightParameters.regularDamageMultiplier;
                break;
            case WeightClassType.Light:
                knockbackMultiplier = WeightParameters.lightKnockbackMultiplier;
                damageMultiplier = WeightParameters.lightDamageMultiplier;
                break;
            case WeightClassType.Heavy:
                knockbackMultiplier = WeightParameters.heavyKnockbackMultiplier;
                damageMultiplier = WeightParameters.heavyDamageMultiplier;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(weightClass), weightClass, null);
        }
    }
}

public enum WeightClassType
{
    Light,
    Default,
    Heavy
} 
