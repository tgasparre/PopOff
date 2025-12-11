using System.Collections.Generic;
using UnityEngine;

public class UltimateAttackHitbox : MonoBehaviour
{
    public Player thisPlayer;
    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        AttackHurtbox hitPlayer = other.GetComponent<AttackHurtbox>();

        if (hitPlayer == thisPlayer.hurtbox)
        {
            return;
        }

        if (hitPlayer != null && !hitPlayers.Contains(hitPlayer))
        {
            hitPlayer.TakeDamage(50);
            hitPlayers.Add(hitPlayer);
        }

        ResetHitPlayers();
    }
        
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }

}
