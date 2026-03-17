using System;
using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private bool hasBeenSteppedOn = false;
    [SerializeField] private SpriteRenderer _render;
    
    private const float startInterval = 0.4f;
    private const float endInterval = 0.05f;

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!hasBeenSteppedOn && other.gameObject.CompareTag("Player"))
        {
            hasBeenSteppedOn = true;
            StartCoroutine(WarningFlash(1.9f));
            StartCoroutine(FallAfterTime(2f));
        }
    }

    IEnumerator FallAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        rb.bodyType = RigidbodyType2D.Dynamic;
        
        //slight delay so player doesn't fall through platform
        yield return new WaitForSeconds(0.2f); 
        GetComponent<BoxCollider2D>().enabled = false;
    }

    IEnumerator WarningFlash(float flashTime)
    {
        float idleTime = 0.2f; 
        Color defaultColor = _render.color;
        yield return new WaitForSeconds(idleTime);
        
        float elapsed = 0f;
        bool isRed = false;
        while (elapsed < flashTime)
        {
            float percentage = elapsed / flashTime;
            float currentInterval = Mathf.Lerp(startInterval, endInterval, percentage);

            isRed = !isRed;
            _render.color = isRed ? Color.white : Color.red;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        _render.color = defaultColor;
    }
}
