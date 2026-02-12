using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public class Explode
{
    [Header("Settings")]
    [SerializeField] private GameObject _explodePrefab;
    [SerializeField] [Tooltip("how long of the lifetime should the explosion be flashing")] private float _flashTimePercent;
    [SerializeField] [Tooltip("how long of the lifetime should the explosion be idle")] private float _idleTimePercent;
    public bool CanExplode => _explodePrefab != null;
    public GameObject Explosion => _explodePrefab;

    private const float startInterval = 0.4f;
    private const float endInterval = 0.05f;
    
    public IEnumerator TriggerExplode(float expireTime, SpriteRenderer renderer, Action callback)
    {
        float idleTime = expireTime * _idleTimePercent; 
        float flashTime = expireTime * _flashTimePercent; 
        yield return new WaitForSeconds(idleTime);
        
        float elapsed = 0f;
        bool isRed = false;
        while (elapsed < flashTime)
        {
            float percentage = elapsed / flashTime;
            float currentInterval = Mathf.Lerp(startInterval, endInterval, percentage);

            isRed = !isRed;
            renderer.color = isRed ? Color.white : Color.red;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        renderer.color = Color.white;
        callback.Invoke();
    }
}
