using UnityEngine;
[System.Serializable]
public struct WeightClass
{
    public string type;
    public float knockbackMultiplier;
    public float damageMultiplier;

    public WeightClass(string weightClass, float knockback, float damage)
    {
        type = weightClass;
        knockbackMultiplier = knockback;
        damageMultiplier = damage;
    }

    public void ChangeWeightClass(string weightClass)
    {
        type = weightClass;
        
        if (weightClass == "regular")
        {
            knockbackMultiplier = WeightParameters.regularKnockbackMultiplier;
            damageMultiplier = WeightParameters.regularDamageMultiplier;
            
        }
        else if (weightClass == "light")
        {
            knockbackMultiplier = WeightParameters.lightKnockbackMultiplier;
            damageMultiplier = WeightParameters.lightDamageMultiplier;
        }
        else if (weightClass == "heavy")
        {
            knockbackMultiplier = WeightParameters.heavyKnockbackMultiplier;
            damageMultiplier = WeightParameters.heavyDamageMultiplier;
        }
    }
}
