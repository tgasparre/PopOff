using System;
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

    public void UpdateText(Weight weight)
    {
        string label;
        Color color;
        (label, color) = GetWeightInfo(weight);
        _weightText.text = label;
        _weightText.color = color;
        _weightEffect.StartManualEffects();
    }

    private (string, Color) GetWeightInfo(Weight w)
    {
        return w switch
        {
            Weight.Light => (_weightLabels[0], _weightColors[0]),
            Weight.Default => (_weightLabels[1], _weightColors[1]),
            Weight.Heavy => (_weightLabels[2], _weightColors[2]),
            _ => throw new ArgumentOutOfRangeException(nameof(w), w, null)
        };
    }

    public enum Weight
    {
        Light,
        Default,
        Heavy
    }
}
