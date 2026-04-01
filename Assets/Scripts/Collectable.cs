using System;
using UnityEngine;

public abstract class Collectable : MonoBehaviour
{
    public event Action OnDeath;
    
    public void Spawned(float timeAlive)
    {
        Destroy(gameObject, timeAlive);
    }

    private void OnDestroy()
    {
        OnDeath?.Invoke();
        OnDeath = null;
    }

    protected abstract void OnTriggerEnter2D(Collider2D other);
}
