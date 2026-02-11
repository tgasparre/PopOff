using System;
using UnityEngine;

public class Dart : MonoBehaviour
{
    [Header("Dart Settings")]
    [SerializeField] private float _rotateSpeed = 4f;
    [SerializeField] private float _lifetime = 2f;
    
    [Header("Optional")]
    [SerializeField] private Explode _explode;
    
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider;
    private int _facingDirection;
    private float _speed;
    private GameObject _throwingPlayer;
    private SpriteRenderer _renderer;
    
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }
    
    public void Fire(GameObject throwingPlayer, DartStats stats, int facing)
    {
        _throwingPlayer = throwingPlayer;
        _facingDirection = facing;

        transform.localScale = Vector3.one * stats.Size;
        _rigidbody2D.gravityScale = stats.FalloffSpeed;
        _speed = stats.speed;
    }

    private void Update()
    {
        if (!_rigidbody2D.simulated) return; 
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, _rigidbody2D.linearVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _rotateSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        _rigidbody2D.linearVelocityX = _facingDirection * _speed * 200 * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == _throwingPlayer) return;
        if (other.gameObject.CompareTag("Player"))
        {
            //TODO
            //damage
            return;
        }
        
        _rigidbody2D.simulated = false;
        _boxCollider.enabled = false;
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
