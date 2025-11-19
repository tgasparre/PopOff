
public static class MovementParameters
{
    // for regular movement taken from crown
   public static float turnAroundSpeedMultiplier = 1.5f;
   public static float groundAccelerationTime = 0.1f;
   public static float airAccelerationTime = 0.1f;
   public static float groundSpeed = 25 / 3f;
   public static float airSpeed = 25 / 3f;
   public static float groundDrag = 20;
   public static float airDrag = 5;
   
   // for fast movement - the light weight class has faster movement and more jumping power
   public static float fastTurnAroundSpeedMultiplier = 0.2f;
   public static float fastGroundAccelerationTime = 0.1f;
   public static float fastAirAccelerationTime = 0.1f;
   public static float fastGroundSpeed = 40 / 3f;
   public static float fastAirSpeed = 25 / 3f;
   public static float fastGroundDrag = 20;
   public static float fastAirDrag = 5;
   
   public static float lightJumpHeight = 16.66667f;
   public static float lightJumpEndEarlyForce = 0.6f;
   
   // for slow movement - the heavy weight class has slower movement and less jumping power
   // does more damage than the other classes
   public static float slowTurnAroundSpeedMultiplier = 2f;
   public static float slowGroundAccelerationTime = 0.1f;
   public static float slowAirAccelerationTime = 0.1f;
   public static float slowGroundSpeed = 20 / 3f;
   public static float slowAirSpeed = 25 / 3f;
   public static float slowGroundDrag = 20;
   public static float slowAirDrag = 5;
   
   public static float heavyJumpHeight = 16.66667f;
   public static float heavyJumpEndEarlyForce = 0.6f;
}
