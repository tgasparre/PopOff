using System;
using UnityEngine;

public class UltimateAttackTrigger : MonoBehaviour
{
    private CombatInputHandler _combatInputHandler;

    private void Awake()
    {
        _combatInputHandler = GetComponentInParent<CombatInputHandler>();
    }

    //Called by the ultimate animation to make it hit when the animation hits 
    public void ActivateUltimateAttackHitbox()
    {
        _combatInputHandler.PerformUltimate();
    }
}
