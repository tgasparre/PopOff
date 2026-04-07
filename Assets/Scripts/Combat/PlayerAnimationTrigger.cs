using System;
using UnityEngine;

public class PlayerAnimationTrigger : MonoBehaviour
{
    private CombatInputHandler _combatInputHandler;
    private Player _player;

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
        _combatInputHandler = GetComponentInParent<CombatInputHandler>();
    }

    //Called by the ultimate animation to make it hit when the animation hits 
    public void ActivateUltimateAttackHitbox()
    {
        _combatInputHandler.PerformUltimate();
    }

    //Called by the death animation when it finishes playing
    public void DeathAnimFinished()
    {
        _player.KillPlayer();
    }
}
