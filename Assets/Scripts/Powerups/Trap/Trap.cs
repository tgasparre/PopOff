using System;
using System.Collections;
using UnityEngine;

public class Trap : Throwable
{
    [Header("Trap")]
    [SerializeField] private float _throwForce;
    [SerializeField] private TrapStyle _style;
    
    [Header("Trap Sounds")]
    [SerializeField] private AudioManager.Audio expandAudio;

    private float _growthTime = 0.25f;
    private float _sizePercentage = 2f;

    public override void Throw(GameObject throwingPlayer, PowerupStats powerupStats, int direction)
    {
        TrapStats stats = (TrapStats)powerupStats;
        base.Throw(throwingPlayer, powerupStats, direction);
        
        _rigidbody2D.AddForce(Vector2.right * direction * _throwForce * 100);
    }

    protected override void HitGround()
    {
        HandleStyle();
        base.HitGround();
    }

    private void HandleStyle()
    {
        switch (_style)
        {
            case TrapStyle.Sticky:
                _rigidbody2D.bodyType = RigidbodyType2D.Static;
                StartCoroutine(GrowInSize());
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
        AudioManager.PlaySound(expandAudio);
        
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
}

public enum TrapStyle
{
    Sticky,
    Bouncy,
}
