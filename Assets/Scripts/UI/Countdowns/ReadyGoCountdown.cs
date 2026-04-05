using System.Collections;
using UnityEngine;

public class ReadyGoCountdown : CountdownUI
{
    [Header("Ready Go Settings")] 
    [SerializeField] private float _showTextDelay = 0.3f;
    [SerializeField] private float _readyWaitTime = 1.8f;
    [SerializeField] private float _goWaitTime = 0.8f;
    [SerializeField] private float _goHoldTime = 0.25f;

    [Space] 
    [SerializeField] private GameObject _readyText;
    [SerializeField] private GameObject _goText;

    private CanvasGroup _readyGroup;
    private CanvasGroup _goGroup;

    protected new void Awake()
    {
        base.Awake();
        _readyGroup = _readyText.GetComponent<CanvasGroup>();
        _goGroup = _goText.GetComponent<CanvasGroup>();
    }
    
    protected override IEnumerator Countdown(float delay)
    {
        SetActiveAll(false);
        yield return new WaitForSeconds(delay);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        isRunning = true;
       SetActiveDelay(_readyGroup, true);
        yield return new WaitForSeconds(_readyWaitTime * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        _readyText.SetActive(false);
        SetActiveDelay(_goGroup, true);
        yield return new WaitForSeconds((_goWaitTime + _goHoldTime) * _timescaleSpeed);
        if (_forceQuit)
        {
            StopCountdownNoTrigger();
            yield break;
        }

        SetActiveAll(false);
        StopCountdown();
    }

    private void SetActiveAll(bool value)
    {
        _readyText.SetActive(value);
        _goText.SetActive(value);
    }

    private void SetActiveDelay(CanvasGroup group, bool value)
    {
        group.alpha = (value) ? 0f : 1f;
        group.gameObject.SetActive(!group.gameObject.activeSelf);
        StartCoroutine(Visible());
        return;

        IEnumerator Visible()
        {
            yield return new WaitForSeconds(_showTextDelay);
            
            group.alpha = (value) ? 1f : 0f;
            group.gameObject.SetActive(value);
        }
    }

    public override void StopCountdownNoTrigger()
    {
        SetActiveAll(false);
        base.StopCountdownNoTrigger();
    }
}
