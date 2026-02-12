using System;
using UnityEngine;

public class ExplodeObject : MonoBehaviour
{
    [SerializeField] private float _explosionLength = 1f;
    
    private void Awake()
    {
        Invoke(nameof(Die), _explosionLength);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
