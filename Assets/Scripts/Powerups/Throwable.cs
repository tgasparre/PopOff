using System;
using System.Collections;
using UnityEngine;

public abstract class Throwable : MonoBehaviour
{
    [Header("Throwable Settings")]
    [SerializeField] protected float _lifetime;
    [SerializeField] protected PhysicsMaterial2D _bouncyMat;
    
    [Header("Explosion Settings")]
    [SerializeField] private Explode _explode;
    
    private PowerupStats.PowerupType type;
    private int _damage = 20;
    private float _glueDuration = 1f;

    protected Rigidbody2D _rigidbody2D;
    protected float _direction;

    protected Player _throwingPlayer;
    private int _throwingPlayerIndex => _throwingPlayer.PlayerIndex;
    
    private SpriteRenderer _renderer;
    private bool _interactable = true;
    private float _interactableTimer = 0.25f;

    protected void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        _throwingPlayer = throwingPlayer.GetComponentInChildren<Player>();
        _direction = direction;

        _renderer.flipX = _direction == 1;

        type = powerupStats.type;
        _damage = powerupStats.damage;
        _glueDuration = powerupStats.glueDuration;
        transform.localScale = Vector3.one * powerupStats.size;

        if (type == PowerupStats.PowerupType.Glue)
        {
            _interactable = false;
            Invoke(nameof(EnableInteraction), _interactableTimer);
        }
        
        Invoke(nameof(Despawn), 40);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Climbable") || other.gameObject.CompareTag("PowerupValid")) HitGround();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player hitPlayer = other.GetComponentInParent<Player>();
            if (!_interactable) return;
            if (hitPlayer.PlayerIndex == _throwingPlayerIndex && type != PowerupStats.PowerupType.Glue) return;

            HitPlayer(hitPlayer);
        }
    }

    private void HitPlayer(Player hitPlayer)
    {
        switch (type)
        {
            case PowerupStats.PowerupType.Damage:
                hitPlayer.TakeDamage(_damage);
                Vector2 knockback = new Vector2(_rigidbody2D.linearVelocityX, -_rigidbody2D.linearVelocityY).normalized;
                hitPlayer.ApplyKnockback(knockback, CombatParameters.KNOCKBACK_FORCE); 
                break;
            case PowerupStats.PowerupType.Glue:
                hitPlayer.FreezePlayer(_glueDuration);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected virtual void HitGround()
    {
        AudioManager.PlaySound(AudioTrack.PowerupThud);
        
        if (_explode.CanExplode) StartCoroutine(_explode.TriggerExplode(_lifetime, _renderer, SpawnExplosion));
        else Invoke(nameof(Despawn), _lifetime);
    }
    
    private void SpawnExplosion()
    {
        _throwingPlayer.GetComponent<PlayerPowerups>().SpawnExplosion(_explode, transform);
        Despawn();
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
    
    private void EnableInteraction()
    {
        _interactable = true;
    }
}
