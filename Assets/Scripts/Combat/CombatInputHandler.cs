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
    public GameObject hitbox;
    public GameObject ultimateHitbox;
    
    private bool UltimateAttackEnabled = false;
    [SerializeField] private UltimateAttackTracker tracker;
    
    void Awake()
    {
        if (hitbox != null)
            hitbox.SetActive(false);
    }

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
            PreformAttack();
    }

    private void PreformAttack()
    {
        StartCoroutine(AttackRoutine());
    }

    private void PreformUltimate()
    {
        StartCoroutine(UltimateAttackRoutine());
    }
    
    //TODO: decide how to pass in damage for primary/secondary attack
    //possibly make separate coroutines?
    //use method SetAttackDamage somewhere here to pass in damage parameters (from combat parameters)
    
    //will likely need changes once we get actual attack animations
    IEnumerator AttackRoutine()
    {
        hitbox.SetActive(true);
        
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
        
        hitbox.SetActive(false);
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