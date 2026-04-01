using System;
using UnityEngine;

public class StageHazard : MonoBehaviour
{
    [SerializeField] private int _damage = 5;
    [SerializeField] private Vector2 knockbackDirection = Vector2.up;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player hitPlayer = other.gameObject.GetComponentInParent<Player>();
            hitPlayer.gameObject.GetComponentInChildren<AttackHurtbox>().TakeDamage(_damage);
            
            hitPlayer.ApplyHitStun(CombatParameters.HIT_STUN_DURATION);
            Vector2 directionToPlayer = (other.gameObject.transform.position - transform.position).normalized;
            Vector2 finalDirection = new Vector2(directionToPlayer.x, knockbackDirection.y).normalized;
            hitPlayer.ApplyKnockback(finalDirection, hitPlayer.playerStats.KnockbackMultiplier() * 3.1f);
            
        }
    }
}
