using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public Player thisPlayer;
    private bool hitSuccessful = false;
    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();

    //placeholder value, overwritten in CombatInputHandler 
    private float attackDamage = 20;

    public bool IsSuccessfulHit()
    {
        return hitSuccessful;
    }

    public void ResetSuccessfulHit()
    {
        hitSuccessful = false;
    }

    //apply this player's damage multiplier depending on their weightclass
    public void SetAttackDamage(float damage)
    {
        attackDamage =  damage * thisPlayer.playerStats.WeightClass.damageMultiplier;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with player entered");
        AttackHurtbox hitPlayer = other.GetComponent<AttackHurtbox>();
        
        //dont hit yourself
        if (hitPlayer.player == thisPlayer)
            return;
        
        if (hitPlayer != null && !hitPlayers.Contains(hitPlayer))
        {
            hitPlayer.TakeDamage(attackDamage);
            hitPlayers.Add(hitPlayer);
            hitSuccessful = true;
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }
    
    
}
