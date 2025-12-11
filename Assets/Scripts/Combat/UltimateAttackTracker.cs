using System;
using UnityEngine;

public class UltimateAttackTracker : MonoBehaviour
{
    public event Action OnUltimateAttackUnlocked;
    
    public PlayerUIDisplayer playerUI;
    public int AttacksNeededForUltimate = 5;
    private int currentAttacks = 0;
    
    [SerializeField] private CombatInputHandler combatInputHandler;

    void Start()
    {
        combatInputHandler.OnSuccessfulHit += OnSuccessfulHit;
    }

    public void SetPlayerUI(PlayerUIDisplayer playerUIDisplayer)
    {
        playerUI = playerUIDisplayer;
        playerUI.UpdateUI(0);
    }

    public void ResetTracker()
    {
        currentAttacks = 0;
        playerUI.UpdateUI(0);
    }

    private void OnSuccessfulHit()
    {
        if (playerUI == null)
        {
            return;
        }
        
        if (currentAttacks >= AttacksNeededForUltimate)
        {
            UnlockUltimateAttack();
        }
        else
        {
            IncrementUltimateAttackCounter();
        }
        
        playerUI.UpdateUI(currentAttacks);
    }
    
    private void UnlockUltimateAttack()
    {
        OnUltimateAttackUnlocked?.Invoke();
        
    }
    
    private void IncrementUltimateAttackCounter()
    {
        if (currentAttacks < AttacksNeededForUltimate)
            ++currentAttacks;
    }
    
    //unsubscribe to OnSuccessfulHit
    void OnDestroy()
    {
        combatInputHandler.OnSuccessfulHit -= OnSuccessfulHit;
    }
    
}
