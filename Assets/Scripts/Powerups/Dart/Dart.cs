using System;
using UnityEngine;

public class Dart : Throwable
{
    [Header("Dart Settings")]
    [SerializeField] private float _rotateSpeed = 4f;
    
    private BoxCollider2D _boxCollider;
    private float _speed;
    
    protected new void Awake()
    {
        base.Awake();
        _boxCollider = GetComponent<BoxCollider2D>();
    }

    public override void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        DartStats stats = (DartStats)powerupStats;
        base.Throw(throwingPlayer, powerupStats, direction);
        
        _rigidbody2D.gravityScale = stats.falloffSpeed;
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
        _rigidbody2D.linearVelocityX = _direction * _speed * 200 * Time.fixedDeltaTime;
    }

    protected override void HitGround()
    {
        _rigidbody2D.simulated = false;
        _boxCollider.enabled = false;
       
        base.HitGround();
    }
}
