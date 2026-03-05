using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinMinigameEnder : MonoBehaviour
{
    public event Action<int> OnCollected;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected.Invoke(other.transform.parent.GetComponent<Player>().PlayerIndex);
            OnCollected = null;
            Destroy(gameObject);
        }
        
    }
}
