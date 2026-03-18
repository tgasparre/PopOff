using System;
using ControllerSystem.Platformer2D;
using UnityEngine;

public class SlipperyPlatform : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        //set player drag to 0 to make it slippery?
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlatformerHorizontalMovementModule>().SetMovementToSlippery();
        }
    }
}
