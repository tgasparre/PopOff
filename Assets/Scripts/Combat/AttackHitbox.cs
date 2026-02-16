using System;
using System.Collections;
using System.Collections.Generic;
using InputManagement;
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
        AttackHurtbox otherHb = other.GetComponent<AttackHurtbox>();
        Player hitPlayer = other.gameObject.GetComponentInParent<Player>();
        
        //dont hit yourself
        if (otherHb.player == thisPlayer)
            return;
        
        InputManager attackerInput = GetComponentInParent<InputManager>();
        
        if (otherHb != null && !hitPlayers.Contains(otherHb))
        {
            otherHb.TakeDamage(attackDamage);
            hitPlayers.Add(otherHb);
            hitPlayer.ApplyKnockback(attackerInput.GetMoveInput(),
                1f);
            
            hitSuccessful = true;
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }
    
    
}
