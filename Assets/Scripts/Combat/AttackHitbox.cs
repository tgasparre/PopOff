using System;
using System.Collections.Generic;
using InputManagement;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

public class AttackHitbox : MonoBehaviour
{
    private Player _player;
    private int _attackDamage;
    private float _knockbackForce;
    private Action onHitCallback;
    
    //particle system variables
    private bool _useParticles;
    [SerializeField] private ParticleSystem attackParticlesPrefab;
    private ParticleSystem attackParticles;
    

    private HashSet<AttackHurtbox> hitPlayers = new HashSet<AttackHurtbox>();

    public void SpawnHitbox(Player player, HitboxType type, Action hitCallback)
    {
        _player = player;
        switch (type)
        {
            case HitboxType.Regular:
                _attackDamage = CombatParameters.BASIC_ATTACK_DMG;
                _knockbackForce = CombatParameters.KNOCKBACK_FORCE;
                _useParticles = true;
                break;
            case HitboxType.Ultimate:
                _attackDamage = CombatParameters.ULTIMATE_ATTACK_DMG;
                _knockbackForce = CombatParameters.ULTIMATE_KNOCKBACK_FORCE;
                _useParticles = false;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
        _attackDamage = Mathf.RoundToInt(_attackDamage * player.playerStats.DamageMultiplier());
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
        
        //on successful hit
        if (otherHb != null && !hitPlayers.Contains(otherHb))
        {
            AddKnockbackAndHitstunToAttacker(_player, attackerInput);
            
            //apply hitstun to hit player and take damage
            hitPlayer.ApplyHitStun(CombatParameters.HIT_STUN_DURATION);
            otherHb.TakeDamage(_attackDamage);
            hitPlayers.Add(otherHb);

            //apply knockback to hit player
            Vector2 direction = (hitPlayer.transform.position - _player.transform.position).normalized;
            
            //add particle effects
            if (_useParticles)
                SpawnAttackParticles(direction);
            
            direction += Vector2.up * attackerInput.GetMoveInput().y;
            hitPlayer.ApplyKnockback(direction, _knockbackForce);
            
            onHitCallback?.Invoke();
        }
    }

    private void AddKnockbackAndHitstunToAttacker(Player hitPlayer, InputManager attackerInput)
    {
        _player.ApplyHitStun(CombatParameters.HIT_STUN_DURATION - 0.2f);
        
        Vector2 direction = (_player.transform.position - hitPlayer.transform.position).normalized;
        direction += Vector2.up * attackerInput.GetMoveInput().y;
        hitPlayer.ApplyKnockback(direction, _knockbackForce/2);
    }

    private void SpawnAttackParticles(Vector2 attackDirection)
    {
        Quaternion spawnRotation = Quaternion.FromToRotation(Vector2.right, attackDirection);
        attackParticles = Instantiate(attackParticlesPrefab, transform.position, spawnRotation);
    }

    public enum HitboxType
    {
        Regular,
        Ultimate
    }
}
