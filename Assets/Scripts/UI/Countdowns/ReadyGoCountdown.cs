using System.Collections;
using UnityEngine;

public class ReadyGoCountdown : CountdownUI
{
    [Header("Ready Go Settings")]
    [SerializeField] private float _readyWaitTime = 1.8f;
    [SerializeField] private float _goWaitTime = 0.8f;

    public override void InitializeCountdown(int value)
    {
        // _countdownText.text = "";
    }

    protected override IEnumerator Countdown(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        isRunning = true;
        // _countdownText.text = "Ready";
        yield return new WaitForSeconds(_readyWaitTime * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        // _countdownText.text = "Go";
        yield return new WaitForSeconds(_goWaitTime * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        StopCountdown();
    }
}
