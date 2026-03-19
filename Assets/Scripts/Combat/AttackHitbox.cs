using System;
using System.Collections.Generic;
using InputManagement;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class AttackHitbox : MonoBehaviour
{
    private Player _player;
    private float _attackDamage;
    private float _knockbackForce;
    private Action onHitCallback;

    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();

    public void SpawnHitbox(Player player, HitboxType type, Action hitCallback)
    {
        _player = player;
        switch (type)
        {
            case HitboxType.Regular:
                _attackDamage = CombatParameters.basicAttackDamage;
                _knockbackForce = CombatParameters.knockbackForce;
                break;
            case HitboxType.Ultimate:
                _attackDamage = CombatParameters.ultimateAttackDamage;
                _knockbackForce = CombatParameters.ultimateKnockbackForce;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        _attackDamage *= player.playerStats.DamageMultiplier();
        _knockbackForce *= player.playerStats.KnockbackMultiplier();
        
        onHitCallback = hitCallback;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        //apply hitstun  to attacking player
        
        AttackHurtbox otherHb = other.GetComponent<AttackHurtbox>();
        Player hitPlayer = other.GetComponentInParent<Player>();
        
        //dont hit yourself
        if (otherHb.player == _player) return;
        InputManager attackerInput = GetComponentInParent<InputManager>();
        
        AddKnockbackAndHitstunToAttacker(_player, attackerInput);
        
        //on successful hit
        if (otherHb != null && !hitPlayers.Contains(otherHb))
        {
            //apply hitstun to hit player and take damage
            hitPlayer.ApplyHitStun(CombatParameters.hitStunDuration);
            otherHb.TakeDamage(_attackDamage);
            hitPlayers.Add(otherHb);

            //apply knockback to hit player
            Vector2 direction = (hitPlayer.transform.position - _player.transform.position).normalized;
            direction += Vector2.up * attackerInput.GetMoveInput().y;
            hitPlayer.ApplyKnockback(direction, _knockbackForce);
            
            onHitCallback?.Invoke();
        }
    }

    private void AddKnockbackAndHitstunToAttacker(Player hitPlayer, InputManager attackerInput)
    {
        _player.ApplyHitStun(CombatParameters.hitStunDuration - 0.2f);
        
        Vector2 direction = (_player.transform.position - hitPlayer.transform.position).normalized;
        direction += Vector2.up * attackerInput.GetMoveInput().y;
        hitPlayer.ApplyKnockback(direction, _knockbackForce/2);
    }

    public enum HitboxType
    {
        Regular,
        Ultimate
    }
}
