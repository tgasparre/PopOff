using System;
using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Trap")]
    [SerializeField] private float _throwForce;
    [SerializeField] private TrapStyle _style;
    [SerializeField] private PhysicsMaterial2D _bouncyMat;
    
    [Header("Optional")]
    [SerializeField] private Explode _explode;

    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _renderer;

    private GameObject _throwingPlayer;
    private float _lifetime;
    
    private bool _hasLanded = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void Throw(GameObject throwingPlayer, float lifetime, int damage, int direction)
    {
        _throwingPlayer = throwingPlayer;
        _lifetime = lifetime;
        _rigidbody2D.AddForce(Vector2.right * direction * _throwForce * 100);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_hasLanded && other.gameObject.CompareTag("Climbable"))
        {
            HandleStyle();
            _hasLanded = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            //TODO
            //deal damage
        }
    }

    private void HandleStyle()
    {
        switch (_style)
        {
            case TrapStyle.Sticky:
                _rigidbody2D.bodyType = RigidbodyType2D.Static;
                break;
            case TrapStyle.Bouncy:
                _rigidbody2D.sharedMaterial = _bouncyMat;
                break;
        }

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

public enum TrapStyle
{
    Sticky,
    Bouncy,
}
