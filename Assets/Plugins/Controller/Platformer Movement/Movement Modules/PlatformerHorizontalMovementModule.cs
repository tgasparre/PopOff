using System;
using UnityEngine;

namespace ControllerSystem.Platformer2D
{
    public class PlatformerHorizontalMovementModule : PlatformerMotorModule
    {
            
        [Tooltip("Higher values makes changing directions feel less slippery")]
        [SerializeField] private float _turnAroundSpeedMultiplier = MovementParameters.turnAroundSpeedMultiplier;
        [SerializeField] private float _groundAccelerationTime = MovementParameters.groundAccelerationTime;
        [SerializeField] private float _airAccelerationTime = MovementParameters.airAccelerationTime;
        [SerializeField] private float _groundSpeed = MovementParameters.groundSpeed;
        [SerializeField] private float _airSpeed = MovementParameters.airSpeed;
        [Tooltip("Applies only when not inputting movement")]
        [SerializeField] private float _groundDrag = MovementParameters.groundDrag;
        [Tooltip("Applies only when not inputting movement")]
        [SerializeField] private float _airDrag = MovementParameters.airDrag;

        private PlatformerCrouchModule _crouchModule;
        
        private void Awake()
        {
            _crouchModule = GetComponent<PlatformerCrouchModule>();
        }

        public override void HandleMovement()
        {
            if (Controller.InputtingHorizontalMovement && (_crouchModule == null || !_crouchModule.Crouching)) // Stop movement if crouching
            {
                float targetSpeed = motor.Grounded ? _groundSpeed : _airSpeed;
                float acceleration = motor.Grounded ? _groundAccelerationTime : _airAccelerationTime;

                float targetVelocity = targetSpeed * Mathf.Sign(Controller.Input.move.GetValue().x);
                float differenceInVelocity = targetVelocity - motor.Rb.linearVelocity.x;

                float horizontalForce = differenceInVelocity / acceleration;

                // Boost force when turning around
                if (!Mathf.Approximately(Mathf.Sign(Controller.Input.move.GetValue().x), Mathf.Sign(motor.Rb.linearVelocity.x)))
                {
                    horizontalForce *= _turnAroundSpeedMultiplier;
                }

                motor.Rb.AddForce(new Vector2(horizontalForce * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
                
                
            }
            else
            {
                float dragForce = motor.Rb.linearVelocity.x * -1;

                dragForce *= motor.Grounded ? _groundDrag : _airDrag;

                motor.Rb.AddForce(new Vector2(dragForce * Time.fixedDeltaTime, 0), ForceMode2D.Impulse);
            }
        }

        public void SetMovementTypeToFast()
        {
            _turnAroundSpeedMultiplier = MovementParameters.fastTurnAroundSpeedMultiplier;
            _groundAccelerationTime = MovementParameters.fastGroundAccelerationTime;
            _airAccelerationTime = MovementParameters.fastAirAccelerationTime;
            _groundSpeed = MovementParameters.fastGroundSpeed;
            _airSpeed = MovementParameters.fastAirSpeed;
            _groundDrag = MovementParameters.fastGroundDrag;
            _airDrag = MovementParameters.fastAirDrag;
        }
        
        public void SetMovementTypeToSlow()
        {
            _turnAroundSpeedMultiplier = MovementParameters.slowTurnAroundSpeedMultiplier;
            _groundAccelerationTime = MovementParameters.slowGroundAccelerationTime;
            _airAccelerationTime = MovementParameters.slowAirAccelerationTime;
            _groundSpeed = MovementParameters.slowGroundSpeed;
            _airSpeed = MovementParameters.slowAirSpeed;
            _groundDrag = MovementParameters.slowGroundDrag;
            _airDrag = MovementParameters.slowAirDrag;
        }

        public void ResetMovement()
        {
            _turnAroundSpeedMultiplier = MovementParameters.turnAroundSpeedMultiplier;
            _groundAccelerationTime = MovementParameters.groundAccelerationTime;
            _airAccelerationTime = MovementParameters.airAccelerationTime;
            _groundSpeed = MovementParameters.groundSpeed;
            _airSpeed = MovementParameters.airSpeed;
            _groundDrag = MovementParameters.groundDrag;
            _airDrag = MovementParameters.airDrag;
        }

        public void SetMovementSpeed(float speed)
        {
            _groundSpeed = speed;
        }

        public float GetMovementSpeed()
        {
            return _groundSpeed;
        }
    }
}