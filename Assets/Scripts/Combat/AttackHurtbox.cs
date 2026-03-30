using System;
using UnityEngine;

public class AttackHurtbox : MonoBehaviour
{
    public Player player;
    public float HP { get; set; }

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(float damage)
    {
        if (player != null) 
            player.TakeDamage(damage);
    }

    public void ResetHealth()
    {
        HP = CombatParameters.MAX_PLAYER_HEALTH;
    }

    public void InstantDeath()
    {
        player.KillPlayer();
    }
    
}
