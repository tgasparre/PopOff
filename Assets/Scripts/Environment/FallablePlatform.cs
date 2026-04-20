using System;
using System.Collections;
using UnityEngine;

public class FallablePlatform : MonoBehaviour
{
    private Collider2D _platformCollider;
    private void Awake()
    {
        _platformCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlatformerCrouchModule crouch = other.gameObject.GetComponentInParent<PlatformerCrouchModule>();
            if (crouch.Crouching)
            {
                crouch.StartPlatformFall(_platformCollider);
            }
        }
    }
}
