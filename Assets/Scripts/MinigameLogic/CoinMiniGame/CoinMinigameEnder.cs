using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoinMinigameEnder : MonoBehaviour
{
    public event Action<int> OnCollected;
    
    private const float AMP = 0.1f;
    private const float FREQ = 2f;
    private Vector3 _startingPos;

    private void Awake()
    {
        _startingPos = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnCollected.Invoke(other.transform.parent.GetComponent<Player>().PlayerIndex);
            OnCollected = null;
            Destroy(gameObject);
        }
        
    }
    
    private void Update()
    {
        float xPos = AMP * Mathf.Sin(FREQ * Time.time);
        float yPos = AMP * Mathf.Sin(FREQ * Time.time);
        transform.position = _startingPos + new Vector3(xPos, yPos);
    }
}
