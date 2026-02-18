using System.Collections.Generic;
using InputManagement;
using UnityEngine;

public class UltimateAttackHitbox : MonoBehaviour
{
    public Player thisPlayer;
    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AttackHurtbox otherHurtbox = other.GetComponent<AttackHurtbox>();
        Player hitPlayer = other.gameObject.GetComponentInParent<Player>();

        if (otherHurtbox == thisPlayer.hurtbox)
        {
            return;
        }
        
        InputManager attackerInput = GetComponentInParent<InputManager>();
        if (otherHurtbox != null && !hitPlayers.Contains(otherHurtbox))
        {
            hitPlayer.ApplyHitStun(CombatParameters.hitStunDuration);
            hitPlayer.ApplyKnockback(attackerInput.GetMoveInput(), CombatParameters.ultimateKnockbackForce);
            otherHurtbox.TakeDamage(50);
            hitPlayers.Add(otherHurtbox);
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }

}
