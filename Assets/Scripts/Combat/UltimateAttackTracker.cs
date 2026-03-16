using System;
using UnityEngine;

public class UltimateAttackTracker : MonoBehaviour
{
    public event Action OnUltimateAttackUnlocked;
    public event Action<float, bool> UICallback_OnUltimateAttackChange;
    
    [SerializeField] private int _attacksNeededForUltimate = 5;
    private int _currentAttacks = 0;

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
        if (CurrentAttacks >= _attacksNeededForUltimate) UnlockUltimateAttack();
        else
        {
            if (CurrentAttacks < _attacksNeededForUltimate) ++CurrentAttacks;
        }
    }
    
    private void UnlockUltimateAttack()
    {
        UICallback_OnUltimateAttackChange?.Invoke(_attacksNeededForUltimate, true);
        OnUltimateAttackUnlocked?.Invoke();
    }
}
