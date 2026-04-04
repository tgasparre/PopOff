using System.Collections;
using EasyTextEffects;
using TMPro;
using UnityEngine;

public class ReadyGoCountdown : CountdownUI
{
    [Header("Ready Go Settings")]
    [SerializeField] private float _readyWaitTime = 1.8f;
    [SerializeField] private float _goWaitTime = 0.8f;
    [SerializeField] private float _goHoldTime = 0.25f;
    
    [Space]
    [SerializeField] private TextMeshProUGUI _readyText; 
    [SerializeField] private TextMeshProUGUI _goText;

    private TextEffect _readyEffect;
    private TextEffect _goEffect;

    protected new void Awake()
    {
        base.Awake();
        _readyEffect = _readyText.GetComponent<TextEffect>();
        _goEffect = _goText.GetComponent<TextEffect>();
    }

    protected override IEnumerator Countdown(float delay)
    {
        _readyText.enabled = false;
        _goText.enabled = false;

        yield return new WaitForSeconds(delay);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        isRunning = true;
        ReadyActive();
        yield return new WaitForSeconds(_readyWaitTime * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        _readyText.enabled = false;
        GoActive();
        yield return new WaitForSeconds((_goWaitTime + _goHoldTime) * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        _readyText.enabled = false;
        _goText.enabled = false;
        StopCountdown();
    }

    private void ReadyActive()
    {
        _readyText.enabled = true;
        _readyEffect.StartManualEffects();
    }
    
    private void GoActive()
    {
        _goText.enabled = true;
        _goEffect.StartManualEffects();
    }

    public override void StopCountdownNoTrigger()
    {
        _readyText.enabled = false;
        _goText.enabled = false;
        base.StopCountdownNoTrigger();
    }
}
