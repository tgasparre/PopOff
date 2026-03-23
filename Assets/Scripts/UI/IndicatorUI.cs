using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _powerupGroup;
    [SerializeField] private Image _indicator;
    private CanvasGroup _canvasGroup;

    private float _savedPowerupAlpha;

    private const float INDICATION_TIMER_LIMIT = 60f;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    public void SetColor(Color color)
    {
        _indicator.color = color;
    }

    public void StartIndicator()
    {
        StartCoroutine(Indication());
        return;

        IEnumerator Indication()
        {
            EnableIndicator();
            Color startColor = _indicator.color;
            Coroutine pulser = StartCoroutine(Pulse(startColor));
            yield return new WaitForSeconds(1f);
            yield return new WaitWhile(() => Game.IsPlayersFrozen);
            yield return new WaitForSeconds(.5f);
            StopCoroutine(pulser);
            
            _indicator.color = startColor;
            DisableIndicator();
        }

        IEnumerator Pulse(Color startColor)
        {
            float elapsed = 0f;
            
            Vector3 pulse;
            Color.RGBToHSV(startColor, out pulse.x, out pulse.y, out pulse.z);

            float baseValue = pulse.z;
            float frequency = 1.2f;

            while (elapsed <= INDICATION_TIMER_LIMIT)
            {
                elapsed += Time.deltaTime;
                
                float t = Mathf.Sin(elapsed * frequency * Mathf.PI * 2f) * 0.5f + 0.5f;
                pulse.z = Mathf.Lerp(baseValue - 0.2f, baseValue, t);
                _indicator.color = Color.HSVToRGB(pulse.x, pulse.y, pulse.z);
                yield return null;
            }
        }
    }

    private void EnableIndicator()
    {
        _savedPowerupAlpha = _powerupGroup.alpha;
        _powerupGroup.alpha = 0f;
        _canvasGroup.alpha = 1f;
    }

    private void DisableIndicator()
    {
        _powerupGroup.alpha = _savedPowerupAlpha;
        _canvasGroup.alpha = 0f;
    }
}
