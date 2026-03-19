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
    private float _damage = 20f;
    private float _glueDuration = 1f;

    protected GameObject _throwingPlayer;
    protected Rigidbody2D _rigidbody2D;
    protected float _direction;
    
    private SpriteRenderer _renderer;
    private bool _interactable = false;
    private float _interactableTimer = 0.1f;

    protected void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
        Invoke(nameof(EnableInteraction), _interactableTimer);
    }

    private void EnableInteraction()
    {
        _interactable = true;
    }

    public virtual void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        _throwingPlayer = throwingPlayer;
        _direction = direction;

        type = powerupStats.type;
        _damage = powerupStats.damage;
        _glueDuration = powerupStats.glueDuration;
        transform.localScale = Vector3.one * powerupStats.size;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Climbable")) HitGround();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_interactable) return;
        if (other.gameObject == _throwingPlayer && type != PowerupStats.PowerupType.Glue) return;
        
        if (other.CompareTag("Player"))
        {  
            Player hitPlayer = other.GetComponentInParent<Player>();
            HitPlayer(hitPlayer);
        }
    }

    private void HitPlayer(Player hitPlayer)
    {
        switch (type)
        {
            case PowerupStats.PowerupType.Damage:
                hitPlayer.TakeDamage(_damage);
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
        if (_explode.CanExplode) StartCoroutine(_explode.TriggerExplode(_lifetime, _renderer, SpawnExplosion));
        else Invoke(nameof(Despawn), _lifetime);
    }
    
    private void SpawnExplosion()
    {
        _throwingPlayer.GetComponent<PlayerPowerups>().SpawnExplosion(_explode.Explosion, transform);
        Despawn();
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
