
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

   public const int defaultDoubleJumps = 1;
   public const float defaultJumpHeight = 16.66667f;
   public const float defaultJumpEndEarlyForce = 0.6f;
   
   // for fast movement - the light weight class has faster movement and more jumping power
   public const float fastTurnAroundSpeedMultiplier = .95f;
   public const float fastGroundAccelerationTime = 0.1f;
   public const float fastAirAccelerationTime = 0.1f;
   public const float fastGroundSpeed = 40 / 3f;
   public const float fastAirSpeed = 45 / 3f;
   public const float fastGroundDrag = 20;
   public const float fastAirDrag = 5;
   
   public const int lightDoubleJumps = 2;
   public const float lightJumpHeight = 16.66667f;
   public const float lightJumpEndEarlyForce = 0.6f;
   
   // for slow movement - the heavy weight class has slower movement and less jumping power
   // does more damage than the other classes
   public const float slowTurnAroundSpeedMultiplier = 2f;
   public const float slowGroundAccelerationTime = 0.1f;
   public const float slowAirAccelerationTime = 0.1f;
   public const float slowGroundSpeed = 23 / 3f;
   public const float slowAirSpeed = 25 / 3f;
   public const float slowGroundDrag = 20;
   public const float slowAirDrag = 6;
   
   public const int heavyDoubleJumps = 1;
   public const float heavyJumpHeight = 16.66667f;
   public const float heavyJumpEndEarlyForce = 0.6f;

   public const float PLATFORM_DROP_TIME = 0.5F;
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
}
