using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public InputManager InputManager;
    public Sprite CombatSprite;
    public Sprite DefaultSprite;
    public AttackHitbox hitbox;

    private void Awake()
    {
        
    }

    void Update()
    {
        
    }

    public void HandleInput(GameObject target)
    {
        if (InputManager.Input.primary.TryUseBuffer())
        {
            Debug.Log("Combat input detected");
            ExecuteAttack();
            RunAttackSpriteAnimation();
        }
    }

    private void ExecuteAttack()
    {
        hitbox.gameObject.SetActive(true);
        Debug.Log("Attack Executed");
    }

    private void DeactivateHitbox()
    {
        hitbox.gameObject.SetActive(false);
    }
    
    //switch to the punching sprite, wait for a small delay then reset the sprite
    private void RunAttackSpriteAnimation()
    {
        ChangeToCombatSprite();
        StartCoroutine(SpriteDelayThenReset());
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

    IEnumerator SpriteDelayThenReset()
    {
        yield return new WaitForSeconds(0.2f);
        ResetSprite();
        DeactivateHitbox();
    }
}