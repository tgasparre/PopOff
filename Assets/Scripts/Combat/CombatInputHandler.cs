using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public InputManager InputManager;
    public Sprite CombatSprite;
    public Sprite DefaultSprite;
    public GameObject hitbox;
    
    void Awake()
    {
        if (hitbox != null)
            hitbox.SetActive(false);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            PreformAttack();
    }

    public void PreformAttack()
    {
        StartCoroutine(AttackRoutine());
    }
    
    private void ChangeToCombatSprite()
    {
        //the sprite renderer exists in the player's child object "Sprite"
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = CombatSprite;
    }
    
    private void ResetSprite()
    {
        gameObject.GetComponentInChildren<SpriteRenderer>().sprite = DefaultSprite;
    }

    IEnumerator AttackRoutine()
    {
        hitbox.SetActive(true);
        ChangeToCombatSprite();
        
        yield return new WaitForSeconds(0.2f);
        
        hitbox.SetActive(false);
        ResetSprite();
    }
}