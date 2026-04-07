using System;
using UnityEngine;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private ReadyGoCountdown _combatCountdown;
    public CanvasGroup Group { get; private set; }

    private void Awake()
    {
        Group = GetComponent<CanvasGroup>();
    }

    public void StartCombatCountdown(Action onFinished)
    {
        ResetCountdown();
        _combatCountdown.StartCountdown(DisableUI, 0.25f);
        return;

        void DisableUI()
        {
            CanvasGroupDisplayer.Hide(Group);
            onFinished.Invoke();
        }
    }

    public void ResetCountdown()
    {
        _combatCountdown.StopCountdownNoTrigger();
    }
}
