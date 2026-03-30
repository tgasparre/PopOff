using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class CameraShake : MonoBehaviour
{
    [Header("Player Hit Shake")]
    [SerializeField] private float _hitDuration = 0.1f;
    [SerializeField] private Vector2 _hitMagnitude = Vector2.one;
    
    [Header("Player Death Shake")]
    [SerializeField] private float _deathDuration = 0.1f;
    [SerializeField] private Vector2 _deathMagnitude = Vector2.one;

    private Camera _currentCamera;
    private Vector3 _currentCameraStartingPosition;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        StopAllCoroutines();
        
        _currentCamera = Camera.main;
        _currentCameraStartingPosition = Camera.main.transform.localPosition;
    }
    
    public void HitShake()
    {
        Shake(_hitDuration, _hitMagnitude);
    }

    public void DeathShake()
    {
        Shake(_deathDuration, _deathMagnitude);
    }
    
    public void Shake(float duration, Vector2 magnitude)
    {
        StartCoroutine(StartShake(duration, magnitude));
    }

    IEnumerator StartShake(float duration, Vector2 magnitude)
    {
        Vector3 originalPos = _currentCameraStartingPosition;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float xOffset = Random.Range(-0.5f, 0.5f) * magnitude.x + originalPos.x;
            float yOffset = Random.Range(-0.5f, 0.5f) * magnitude.y + originalPos.y;
            Camera.main.transform.localPosition = new Vector3(xOffset, yOffset, originalPos.z);

            yield return null;
        }
        Camera.main.transform.localPosition = originalPos;
    }
}
