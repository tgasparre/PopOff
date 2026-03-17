using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStart : PlayerBase
{
    [Space]
    [SerializeField] private float _movementSpeed = 8f;
    [SerializeField] private float _boostForce = 5f;
    [SerializeField] private float _boostInterval = 4f;
    [SerializeField] private GameObject _playerPrefab;

    private GroundCheck _groundCheck;
    private Coroutine _jumpCoroutine = null;
    private Vector2 _moveDirection => _playerInputManager.GetMoveInput();
    
    public bool InputtingHorizontalMovement => Mathf.Abs(_moveDirection.x) > 0.5f;
    
    // private static readonly Vector2 BoostFactor = new Vector2(.8f, 1.2f);
    public Vector2 BoostFactor;

    private new void Awake()
    {
        base.Awake();
        _groundCheck = GetComponentInChildren<GroundCheck>();
    }

    private IEnumerator LaunchJump()
    {
        float timer = 0f;
        do
        {
            timer += Time.fixedDeltaTime;
            _rigidbody2D.linearVelocity = Vector2.zero;
            yield return null;
        } 
        while (timer < 0.23f);

        _rigidbody2D.AddForce(new Vector2(BoostFactor.x * FacingLeftValue, BoostFactor.y) * _boostForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_boostInterval);
        _jumpCoroutine = null;
    }

    private void FixedUpdate()
    { 
        if (!_playerInputManager.isInputEnabled) return;
        HandleMovement();
        HandleJump();
    }

    private void HandleJump()
    {
        if (_groundCheck.Grounded && _playerInputManager.Input.jump.TryUseBuffer())
        {
            _jumpCoroutine ??= StartCoroutine(LaunchJump());
        }    
    }
    
    private void HandleMovement()
    {
        if (_groundCheck.Grounded)
        {
            if (InputtingHorizontalMovement)
            {
                float targetVelocity = _movementSpeed * Mathf.Sign(_moveDirection.x);
                float differenceInVelocity = targetVelocity - _rigidbody2D.linearVelocity.x;

                float horizontalForce = differenceInVelocity / 0.1f;

                // Boost force when turning around
                if (!Mathf.Approximately(Mathf.Sign(_moveDirection.x), Mathf.Sign(_rigidbody2D.linearVelocity.x)))
                {
                    horizontalForce *= 1.5f;
                }

                _rigidbody2D.AddForce(new Vector2(horizontalForce * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }
            else 
            {
                float dragForce = _rigidbody2D.linearVelocity.x * -1;
                dragForce *= 20f;
                _rigidbody2D.AddForce(new Vector2(dragForce * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }
        }
    }

    private void OnDisable()
    {
        _jumpCoroutine = null;
    }
}
