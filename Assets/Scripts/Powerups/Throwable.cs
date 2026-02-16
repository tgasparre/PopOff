using System;
using UnityEngine;

public abstract class Throwable : MonoBehaviour
{
    [Header("Throwable Settings")]
    [SerializeField] protected float _lifetime;
    [SerializeField] protected PhysicsMaterial2D _bouncyMat;
    
    [Header("Explosion Settings")]
    [SerializeField] private Explode _explode;

    protected GameObject _throwingPlayer;
    protected Rigidbody2D _rigidbody2D;
    protected float _direction;
    
    private SpriteRenderer _renderer;

    protected void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public virtual void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        _throwingPlayer = throwingPlayer;
        _direction = direction;
        transform.localScale = Vector3.one * powerupStats.size;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == _throwingPlayer) return;
        if (other.gameObject.CompareTag("Player"))
        {
            //TODO 
            //damage
        }
        else if (other.gameObject.CompareTag("Climbable")) HitGround();
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
