using System;
using System.Collections;
using UnityEngine;

public class Trap : Throwable
{
    [Header("Trap")]
    [SerializeField] private float _throwForce;
    [SerializeField] private TrapStyle _style;

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
                break;
            case TrapStyle.Bouncy:
                _rigidbody2D.sharedMaterial = _bouncyMat;
                break;
        }
    }
}

public enum TrapStyle
{
    Sticky,
    Bouncy,
}
