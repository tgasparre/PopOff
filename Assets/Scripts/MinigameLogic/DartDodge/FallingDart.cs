using System;
using UnityEngine;

public class FallingDart : MonoBehaviour
{
    [SerializeField] private float _lifetime = 1f;

    [SerializeField] private bool _hidden = false;

    private SpriteRenderer _renderer;
    private BoxCollider2D[] _colliders;
    private Rigidbody2D _rigidbody2D;
    private bool _interactable = true;
    private Vector3 _startingPosition;

    private void Awake()
    {
        _startingPosition = transform.position;
        _renderer = GetComponentInChildren<SpriteRenderer>();
        _colliders = GetComponents<BoxCollider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        
        ReturnToPool();
    }

    public void Spawn(Vector2 position, float fallRate)
    {
        EnableComponents(true);
        transform.position = position;
        _rigidbody2D.gravityScale = fallRate;
    }

    public void ReturnToPool()
    {
        EnableComponents(false);
        transform.position = _startingPosition;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Climbable"))
        {
            _interactable = false;
            if (_hidden) ReturnToPool();
            else Invoke(nameof(ReturnToPool), _lifetime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_interactable) return;
        if (other.CompareTag("Player"))
        {  
            Player hitPlayer = other.GetComponentInParent<Player>();
            hitPlayer.InstantDeath();
        }
    }
    
    private void EnableComponents(bool value)
    {
        _interactable = value;
        _renderer.enabled = value;
        _rigidbody2D.simulated = value;
        foreach (BoxCollider2D c in _colliders)
        {
            c.enabled = value;
        }
    }
}
