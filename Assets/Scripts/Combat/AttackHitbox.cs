using System.Collections.Generic;
using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collision with player entered");
        AttackHurtbox hitPlayer = other.GetComponent<AttackHurtbox>();

        if (hitPlayer != null && !hitPlayers.Contains(hitPlayer))
        {
            hitPlayer.TakeDamage(20);
            hitPlayers.Add(hitPlayer);
        }

        ResetHitPlayers();
    }
    
    private void ResetHitPlayers()
    {
        hitPlayers.Clear();
    }
    
    
}
