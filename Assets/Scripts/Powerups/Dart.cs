using System;
using UnityEngine;

public class Dart : MonoBehaviour
{
    [Header("Dart Settings")]
    [SerializeField] private float _maxAngleDistance = 4f;
    [SerializeField] private float _targetAngle = 45;
    [SerializeField] private float _lifetime = 2f;
    [SerializeField] private float _delayTime = 0.88f;
    
    private Rigidbody2D _rigidbody2D;
    private BoxCollider2D _boxCollider;
    private int _facingDirection;
    private float _speed;
    private float TargetAngle => _targetAngle * _facingDirection;

    private bool _canCurve = false;

    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _canCurve = false;
    }
    
    public void Fire(DartStats stats, int facing)
    {
        _facingDirection = facing;
        _speed = stats.speed;
        Invoke(nameof(StartCurveTimer), _delayTime);
    }

    private void StartCurveTimer()
    {
        _canCurve = true;
    }

    private void Update()
    {
        transform.position += (Vector3)(Vector2.right * _facingDirection * _speed * 10 * Time.deltaTime);

        if (!_canCurve) return; 
        Vector2 rayOrigin = new Vector2(transform.position.x, transform.position.y - 0.2f);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, _maxAngleDistance, LayerMask.GetMask("Ground"));
        if (hit)
        {
            float distance = Vector2.Distance(hit.point, transform.position);
            Vector3 temp = transform.localEulerAngles;
            temp.z = Mathf.Lerp(0f, TargetAngle, 1 - distance / 4f);
            transform.localEulerAngles = temp;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        _rigidbody2D.simulated = false;
        _boxCollider.enabled = false;
        Invoke(nameof(Despawn), _lifetime);
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}
