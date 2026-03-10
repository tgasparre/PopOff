
using UnityEngine;

public static class MovementParameters
{
    // for regular movement taken from crown
   public const float turnAroundSpeedMultiplier = 1.5f;
   public const float groundAccelerationTime = 0.1f;
   public const float airAccelerationTime = 0.1f;
   public const float groundSpeed = 25 / 3f;
   public const float airSpeed = 25 / 3f;
   public const float groundDrag = 20;
   public const float airDrag = 5;
   
   // for fast movement - the light weight class has faster movement and more jumping power
   public const float fastTurnAroundSpeedMultiplier = 0.2f;
   public const float fastGroundAccelerationTime = 0.1f;
   public const float fastAirAccelerationTime = 0.1f;
   public const float fastGroundSpeed = 40 / 3f;
   public const float fastAirSpeed = 25 / 3f;
   public const float fastGroundDrag = 20;
   public const float fastAirDrag = 5;
   
   public const float lightJumpHeight = 16.66667f;
   public const float lightJumpEndEarlyForce = 0.6f;
   
   // for slow movement - the heavy weight class has slower movement and less jumping power
   // does more damage than the other classes
   public const float slowTurnAroundSpeedMultiplier = 2f;
   public const float slowGroundAccelerationTime = 0.1f;
   public const float slowAirAccelerationTime = 0.1f;
   public const float slowGroundSpeed = 20 / 3f;
   public const float slowAirSpeed = 25 / 3f;
   public const float slowGroundDrag = 20;
   public const float slowAirDrag = 5;
   
   public const float heavyJumpHeight = 16.66667f;
   public const float heavyJumpEndEarlyForce = 0.6f;
}

[System.Serializable]
public struct CustomPlayerParameters
{
    public float turnAroundSpeed;
    public float groundAcceleration;
    public float airAcceleration;
    public float groundSpeed;
    public float airSpeed;
    public float groundDrag;
    public float airDrag;
    
    public int numDoubleJump;
    public float jumpHeight;
    public float jumpEndEarlyForce;

    public CustomPlayerParameters(float turnAroundSpeed, float groundAcceleration, float airAcceleration,
        float groundSpeed, float airSpeed, float groundDrag, float airDrag, int numDoubleJump, float jumpHeight, float jumpEndEarlyForce)
    {
        this.turnAroundSpeed = turnAroundSpeed;
        this.groundAcceleration = groundAcceleration;
        this.airAcceleration = airAcceleration;
        this.groundSpeed = groundSpeed;
        this.airSpeed = airSpeed;
        this.groundDrag = groundDrag;
        this.airDrag = airDrag;

        this.numDoubleJump = numDoubleJump;
        this.jumpHeight = jumpHeight;
        this.jumpEndEarlyForce = jumpEndEarlyForce;
    }
}
