using System.Collections;
using ControllerSystem.Platformer2D;
using UnityEngine;

public class PlatformerCrouchModule : PlatformerMotorModule
{
    private bool _crouching;
    public bool Crouching => motor.Grounded && _crouching && !_fallThrough;

    private bool _fallThrough;

    [SerializeField] private Collider2D _playerCollider;
    
    public override void HandleMovement()
    {
        _crouching = Controller.Input.move.GetValue().y < -0.65f;
    }

    public void StartPlatformFall(Collider2D platform)
    {
        StartCoroutine(FallThrough());
        return;

        IEnumerator FallThrough()
        {
            _fallThrough = true;
            Physics2D.IgnoreCollision(_playerCollider, platform, true);
            yield return new WaitForSeconds(MovementParameters.PLATFORM_DROP_TIME);
            Physics2D.IgnoreCollision(_playerCollider, platform, false);
            _fallThrough = false;
        }
    }
}
