using System;
using System.Collections;
using UnityEngine;

public class Trap : Throwable
{
    [Header("Trap")]
    [SerializeField] private float _throwForce;
    [SerializeField] private TrapStyle _style;
    [SerializeField] private Sprite _glueImage;
    
    [Header("Glue Particles")]
    [SerializeField] private ParticleSystem _particleSystem;

    private Collision2D _collision;
    
    private float _growthTime = 0.25f;
    private float _sizePercentage = 2f;

    public override void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        TrapStats stats = (TrapStats)powerupStats;
        base.Throw(throwingPlayer, powerupStats, direction);
        
        _rigidbody2D.AddForce(Vector2.right * direction * _throwForce * 100);
    }

    protected override void HitGround(Collision2D collision)
    {
        _collision = collision;
        HandleStyle();
        base.HitGround(collision);
    }

    private void HandleStyle()
    {
        switch (_style)
        {
            case TrapStyle.Sticky:
                _rigidbody2D.bodyType = RigidbodyType2D.Static;
                // StartCoroutine(GrowInSize());
                ActivateGlue();
                break;
            case TrapStyle.Bouncy:
                _rigidbody2D.sharedMaterial = _bouncyMat;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private IEnumerator GrowInSize()
    {
        Vector3 target = transform.localScale * _sizePercentage;
        Vector3 starting = transform.localScale;
        float elapsed = 0f;
        while (elapsed < _growthTime)
        {
            elapsed += Time.deltaTime;
            Vector3 temp = Vector3.Lerp(starting, target, elapsed / _growthTime);
            transform.localScale = temp;
            yield return null;
        }
        transform.localScale = target;
    }

    private void ActivateGlue()
    {
        if (_collision.gameObject.GetComponent<FallingPlatform>() != null)
        {
            transform.parent = _collision.transform;
            _rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(transform.position + (Vector3.up), Vector2.down, 10f, Layers.Default);
        if (hit)
        {
            Vector3 temp = transform.position;
            temp.y = hit.point.y;
            transform.position = temp;
        }
        _renderer.sprite = _glueImage;
        
        /* SPAWN PARTICLES HERE */
    }
}

public enum TrapStyle
{
    Sticky,
    Bouncy,
}
