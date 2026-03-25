using System;
using System.Collections;
using EasyTextEffects;
using TMPro;
using UnityEngine;

public class WeightUI : MonoBehaviour
{
    [SerializeField] private AirFillBoard _fillBoard;
    [Space]
    [SerializeField] private TextMeshProUGUI _weightText;
    private TextEffect _weightEffect;
    
    [Space]
    [SerializeField] private Color[] _weightColors;
    [SerializeField] private string[] _weightLabels;

    private Coroutine _textEffectDelay;

    private CanvasGroup _canvasGroup;
    public bool IsVisible
    {
        get => _canvasGroup.alpha == 1f;
        set => _canvasGroup.alpha = (value) ? 1f : 0f;
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _weightEffect = GetComponentInChildren<TextEffect>();
    }

    private void LateUpdate()
    {
        if (MiniGameInfo.IsPlayingMiniGame)
        {
            UpdateText();
        }
    }
    
    private void UpdateText()
    {
        string label;
        Color color;
        (label, color) = GetWeightInfo(_fillBoard.GetWeight());

        if (label.Equals(_weightText.text)) return;
        _textEffectDelay ??= StartCoroutine(Delay());
        return;

        IEnumerator Delay()
        {
            _weightText.text = label;
            _weightText.color = color;
            _weightEffect.StartManualEffects();   
            yield return new WaitForSeconds(0.7f);
            _textEffectDelay = null;
        }
    }

    private (string, Color) GetWeightInfo(StartingMiniGame.Weight w)
    {
        return w switch
        {
            StartingMiniGame.Weight.Light => (_weightLabels[0], _weightColors[0]),
            StartingMiniGame.Weight.Default => (_weightLabels[1], _weightColors[1]),
            StartingMiniGame.Weight.Heavy => (_weightLabels[2], _weightColors[2]),
            _ => throw new ArgumentOutOfRangeException(nameof(w), w, null)
        };
    }

   
}
