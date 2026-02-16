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

    public GameObject hitboxPrefab;
    public GameObject ultimateHitboxPrefab;

    private Vector2 moveInput;
    
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
            Vector3 attackDirection = GetAttackDirection();
            PreformAttack(attackDirection);
        }
            
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 attackDirection = GetAttackDirection();
            PreformSecondaryAttack(attackDirection);
        }
    }

    private Vector3 GetAttackDirection()
    {
        moveInput = InputManager.GetMoveInput();
        Debug.Log(moveInput);
        switch (moveInput.y)
        {
            case > 0:
                //Debug.Log("Up detected");
                return new Vector3(0, 0.5f, 0);
            case < 0:
                //Debug.Log("Down detected");
                return new Vector3(0,-0.5f,0);
        }
        switch (moveInput.x)
        {
            //TODO: if the player isnt moving, moveinput defaults to 0, need 2 find proper way to tell which direction player is facing
            case > 0:
                return new Vector3(0.5f,0,0);
            case < 0:
                return new Vector3(-1f,0,0);
            //if not moving, use last facing direction
            default:
                break;
        }
        return Vector3.zero;   
    }

    private void PreformAttack(Vector3 offset)
    {
        StartCoroutine(AttackRoutine(offset));
    }

    private void PreformUltimate()
    {
        StartCoroutine(UltimateAttackRoutine());
    }

    private void PreformSecondaryAttack(Vector3 offset)
    {
        StartCoroutine(SecondaryAttackRoutine(offset));
    }
    
    //will likely need changes once we get actual attack animations
    IEnumerator AttackRoutine(Vector3 offset)
    {
        GameObject hitbox = Instantiate(hitboxPrefab, 
            transform.position + offset, 
            Quaternion.identity, 
            transform);
        
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();
        
        hitboxScript.thisPlayer = gameObject.GetComponent<Player>();
        hitboxScript.SetAttackDamage(10);
        
        ChangeToCombatSprite(); // <- change once art is in
        
        yield return new WaitForSeconds(0.2f);
        
        //check to see if the hit was successful to increment ultimate attack meter
        if (hitboxScript.IsSuccessfulHit())
        {
            OnSuccessfulHit?.Invoke();
            hitboxScript.ResetSuccessfulHit();
        }
        //TODO: adjust time once done with testing, 2 seconds is too long for a real hitbox
        Destroy(hitbox, 2f);
        ResetSprite();
    }

    IEnumerator UltimateAttackRoutine()
    {
        GameObject ultimateHitbox = Instantiate(ultimateHitboxPrefab, 
            transform.position, 
            Quaternion.identity, 
            transform);
        ultimateHitbox.GetComponent<UltimateAttackHitbox>().thisPlayer = gameObject.GetComponentInParent<Player>();
        
        ChangeToCombatSprite();
        yield return new WaitForSeconds(0.2f);
        Destroy(ultimateHitbox, 2f);
        ResetSprite();
        
    }

    //I know this is mostly repeat code, but the animations will need to be different than the primary attack
    //so it will be useful to have its own coroutine
    IEnumerator SecondaryAttackRoutine(Vector3 offset)
    {
        GameObject hitbox = Instantiate(hitboxPrefab, 
            transform.position + offset, 
            Quaternion.identity, 
            transform);
        
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();
        
        hitboxScript.thisPlayer = gameObject.GetComponent<Player>();
        hitboxScript.SetAttackDamage(25);
        
        ChangeToCombatSprite();
        
        yield return new WaitForSeconds(0.2f);
        
        //check to see if the hit was successful to increment ultimate attack meter
        if (hitboxScript.IsSuccessfulHit())
        {
            OnSuccessfulHit?.Invoke();
            hitboxScript.ResetSuccessfulHit();
        }
        //TODO: adjust time once done with testing, 2 seconds is too long for a real hitbox
        Destroy(hitbox, 2f);
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