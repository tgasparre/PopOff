using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public InputManager InputManager;
    public Sprite CombatSprite;
    public Sprite DefaultSprite;
    private Command kButton;

    private void Awake()
    {
        kButton = new BasicAttackCommand();
    }

    public void HandleInput(GameObject target)
    {
        if (InputManager.Input.primary.TryUseBuffer())
        {
            RunAttackSpriteAnimation();
            kButton.Execute(target);
        }
    }

    
    //switch to the punching sprite, wait for a small delay then reset the sprite
    private void RunAttackSpriteAnimation()
    {
        ChangeToCombatSprite();
        StartCoroutine(SpriteDelay());
        ResetSprite();
        // StopCoroutine(SpriteDelay());
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

    IEnumerator SpriteDelay()
    {
        yield return new WaitForSeconds(0.2f);
    }
}