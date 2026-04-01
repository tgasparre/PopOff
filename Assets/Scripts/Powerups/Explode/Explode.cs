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
    [SerializeField] private float _explosionRadius = 2f;
    [SerializeField] private float _explosionDamage = 20f;
    [SerializeField] private float _explosionForce = 1000f;

    public float Radius => _explosionRadius;
    public float Damage => _explosionDamage;
    public float Force => _explosionForce;
    
    public bool CanExplode => _explodePrefab != null;
    public GameObject Explosion => _explodePrefab;

    private const float START_INTERVAL = 0.4f;
    private const float END_INTERVAL = 0.05f;
    
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
            float currentInterval = Mathf.Lerp(START_INTERVAL, END_INTERVAL, percentage);

            isRed = !isRed;
            renderer.color = isRed ? Color.white : Color.red;

            yield return new WaitForSeconds(currentInterval);
            elapsed += currentInterval;
        }

        renderer.color = Color.white;
        
        AudioManager.PlaySound(AudioTrack.PowerupExplode);
        callback.Invoke();
    }
}
