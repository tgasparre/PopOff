using System;
using UnityEngine;

public enum Side
{
    Left,
    Right
}

public class TiltingPlatformSensor : MonoBehaviour
{
    public Side side;
    
    public event Action<Side> PlayerEntered;
    public event Action<Side> PlayerExited;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerEntered?.Invoke(side);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerExited?.Invoke(side);
        }
    }
}
