using Unity.VisualScripting;
using UnityEngine;

public class AttackModule : MonoBehaviour
{
    private bool inAttackRange = false;
    [SerializeField]
    private CombatInputHandler combatInputHandler;

    public bool CheckIfInAttackRange()
    {
        return inAttackRange;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        // if the object you're colliding with isn't another player, do nothing
        if (!other.TryGetComponent(out Player player)) 
            return;
        inAttackRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        inAttackRange = false;
    }
    
}
