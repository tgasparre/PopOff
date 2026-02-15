using System;
using System.Collections;
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
        AttackHurtbox otherHurtbox = other.GetComponent<AttackHurtbox>();
        Player hitPlayer = other.gameObject.GetComponentInParent<Player>();
        Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;
        
        //dont hit yourself
        if (otherHurtbox.player == thisPlayer)
            return;
        
        if (otherHurtbox != null && !hitPlayers.Contains(otherHurtbox))
        {
            otherHurtbox.TakeDamage(attackDamage);
            hitPlayer.ApplyKnockback(knockbackDirection, hitPlayer.playerStats.WeightClass.knockbackMultiplier);
            hitPlayers.Add(otherHurtbox);
            hitSuccessful = true;
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }
    
    
}
