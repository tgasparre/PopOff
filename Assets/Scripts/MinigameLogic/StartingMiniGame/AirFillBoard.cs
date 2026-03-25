using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AirFillBoard : MonoBehaviour
{
    [SerializeField] private PIndex _pIndex;

    //===== References =====
    private Slider _progressSlider;
    private CanvasGroup _canvasGroup;
    
    private float _fillRate;
    private float _deflateRate;

    public bool IsVisible
    {
        get => _canvasGroup.alpha == 1f;
        set => _canvasGroup.alpha = (value) ? 1f : 0f;
    }

    public float FillAmount => _progressSlider.value;

    public StartingMiniGame.Weight GetWeight()
    {
        StartingMiniGame.Weight weightClass = StartingMiniGame.Weight.Light;
        if (FillAmount < 0.66f) weightClass = StartingMiniGame.Weight.Default;
        if (FillAmount < 0.33f) weightClass = StartingMiniGame.Weight.Heavy;
        return weightClass;
    }
    
    private void Awake()
    {
        _progressSlider = GetComponentInChildren<Slider>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetValues(float fillRate, float deflateRate)
    {
        _fillRate = fillRate;
        _deflateRate = deflateRate;
    }

    public void Fill(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _progressSlider.value += _fillRate;
        }
    }

    private void Update()
    {
        if (MiniGameInfo.IsPlayingMiniGame)
        {
            _progressSlider.value -= _deflateRate * Time.deltaTime;
        }
    }
    
}
