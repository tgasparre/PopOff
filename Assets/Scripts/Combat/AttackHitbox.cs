using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using InputManagement;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class AttackHitbox : MonoBehaviour
{
    public Player thisPlayer;
    private bool hitSuccessful = false;
    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();

    //placeholder value, overwritten in CombatInputHandler 
    private float attackDamage = 10;

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
        AttackHurtbox otherHb = other.GetComponent<AttackHurtbox>();
        Player hitPlayer = other.GetComponentInParent<Player>();
        
        //dont hit yourself
        if (otherHb.player == thisPlayer)
            return;
        
        Debug.Log("Collision with player entered");
        InputManager attackerInput = GetComponentInParent<InputManager>();
        
        if (otherHb != null && !hitPlayers.Contains(otherHb))
        {
            hitPlayer.ApplyHitStun(CombatParameters.hitStunDuration);
            otherHb.TakeDamage(attackDamage);
            hitPlayers.Add(otherHb);

            Vector2 direction = (hitPlayer.transform.position - thisPlayer.transform.position).normalized;
            direction += Vector2.up * attackerInput.GetMoveInput().y;
            hitPlayer.ApplyKnockback(direction, CombatParameters.knockbackForce);
            
            hitSuccessful = true;
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }
    
    
}
