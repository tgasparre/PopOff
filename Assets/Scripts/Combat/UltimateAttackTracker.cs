using System;
using UnityEngine;

public class UltimateAttackTracker : MonoBehaviour
{
    public event Action OnUltimateAttackUnlocked;
    public event Action<float, bool> UICallback_OnUltimateAttackChange;
    
    [SerializeField] private int _attacksNeededForUltimate = 5;
    private int _currentAttacks = 0;

    private int _attackLimitForIEEE => _attacksNeededForUltimate - 1;

    public int CurrentAttacks
    {
        get => _currentAttacks;
        set
        {
            _currentAttacks = value;
            UICallback_OnUltimateAttackChange?.Invoke((float)value/_attacksNeededForUltimate, false);
        }
    }

    public void ResetTracker()
    {
        CurrentAttacks = 0;
    }

    public void OnSuccessfulHit()
    {
        if (CurrentAttacks == _attackLimitForIEEE) return;
        
        CurrentAttacks++;
        if (CurrentAttacks == _attacksNeededForUltimate) UnlockUltimateAttack();
        
    }
    
    public void UnlockUltimateAttack()
    {
        CurrentAttacks = _attacksNeededForUltimate;
        UICallback_OnUltimateAttackChange?.Invoke(_attacksNeededForUltimate, true);
        OnUltimateAttackUnlocked?.Invoke();
    }
}
