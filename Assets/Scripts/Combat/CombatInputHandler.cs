using System;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public event Action OnSuccessfulHit;
    
    public InputManager InputManager;
    public Sprite CombatSprite;
    public Sprite DefaultSprite;
    public GameObject ultimateHitbox;

    public GameObject hitboxPrefab;

    private Vector2 moveInput;
    private bool facingRight = true;
    
    private bool UltimateAttackEnabled = false;
    [SerializeField] private UltimateAttackTracker tracker;

    void Start()
    {
        tracker.OnUltimateAttackUnlocked += OnUltimateAttackUnlocked;
    }

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (UltimateAttackEnabled)
        {
            PreformUltimate();
            tracker.ResetTracker();
            ResetUltimateAttack();
        }
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveInput = InputManager.GetMoveInput();
            Debug.Log(moveInput);
            if (moveInput.y > 0)
            {
                Debug.Log("Up detected");
                PreformAttack(new Vector3(0,0.5f,0));
            }
            else if (moveInput.y < 0)
            {
                Debug.Log("Down detected");
                PreformAttack(new Vector3(0,-0.5f,0));
            }
            else 
            {
                //do a horizontal attack depending on which way the player is facing
                if (facingRight)
                {
                    PreformAttack(new Vector3(-0.5f,0,0));
                }
                else 
                {
                    PreformAttack(new Vector3(0.5f,0,0));
                }
               
            }
        }
            
    }

    public void OnSecondaryAttack()
    {
        
    }

    private void PreformAttack(Vector3 offset)
    {
        StartCoroutine(AttackRoutine(offset));
    }

    private void PreformUltimate()
    {
        StartCoroutine(UltimateAttackRoutine());
    }
    
    //will likely need changes once we get actual attack animations
    //TODO: must destroy hitbox after certain time (once done with position testing)
    IEnumerator AttackRoutine(Vector3 offset)
    {
        GameObject hitbox = Instantiate(hitboxPrefab, 
            transform.position + offset, 
            Quaternion.identity, 
            transform);
        
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();
        hitboxScript.thisPlayer = gameObject.GetComponent<Player>();
        
        ChangeToCombatSprite();
        
        yield return new WaitForSeconds(0.2f);
        
        //check to see if the hit was successful to increment ultimate attack meter
        if (hitboxScript.IsSuccessfulHit())
        {
            OnSuccessfulHit?.Invoke();
            hitboxScript.ResetSuccessfulHit();
        }
        
        Destroy(hitbox, 2f);
        ResetSprite();
    }

    IEnumerator UltimateAttackRoutine()
    {
        ultimateHitbox.SetActive(true);
        ultimateHitbox.GetComponent<UltimateAttackHitbox>().thisPlayer = gameObject.GetComponent<Player>();
        
        ChangeToCombatSprite();
        yield return new WaitForSeconds(0.2f);
        ultimateHitbox.SetActive(false);
        ResetSprite();
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

    private void OnUltimateAttackUnlocked()
    {
        UltimateAttackEnabled = true;
        Debug.Log("Ultimate attack unlocked!");
    }

    private void ResetUltimateAttack()
    {
        UltimateAttackEnabled = false;
    }

    private void OnDestroy()
    {
        tracker.OnUltimateAttackUnlocked -= OnUltimateAttackUnlocked;
    }

    
}