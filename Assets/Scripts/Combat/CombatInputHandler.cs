using System;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public event Action OnSuccessfulHit;
    
    public InputManager InputManager;
    public GameObject hitboxPrefab;
    public GameObject ultimateHitboxPrefab;

    private Vector2 moveInput;
    [Header("Attack Settings")]
    [SerializeField] private float _hitboxLifetime = 0.5f;
    [Header("Attack Locations")]
    [SerializeField] [Range(0,90)] private float _angleSectorValue = 45;
    [SerializeField] private Transform _upAttack;
    [SerializeField] private Transform _downAttack;
    [SerializeField] private Transform _leftAttack;
    [SerializeField] private Transform _rightAttack;
    
    private bool UltimateAttackEnabled = false;
    [Space] [SerializeField] private UltimateAttackTracker tracker;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    void Start()
    {
        tracker.OnUltimateAttackUnlocked += OnUltimateAttackUnlocked;
    }

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (!InputManager.isInputEnabled) return;
        if (UltimateAttackEnabled)
        {
            PreformUltimate();
            tracker.ResetTracker();
            ResetUltimateAttack();
        }
    }

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (!InputManager.isInputEnabled) return;
        if (context.performed)
        {
            Vector3 attackDirection = GetAttackDirection();
            PreformAttack(attackDirection);
        }
            
    }

    public void OnSecondaryAttack(InputAction.CallbackContext context)
    {
        if (!InputManager.isInputEnabled) return;
        if (context.performed)
        {
            Vector3 attackDirection = GetAttackDirection();
            PreformSecondaryAttack(attackDirection);
        }
    }

    private Vector3 GetAttackDirection()
    {
        moveInput = InputManager.GetMoveInput();
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        if (angle == 0)
        {
            return InputManager.GetFacingDirection() switch
            {
                -1 => _leftAttack.position,
                1 => _rightAttack.position,
                _ => Vector3.zero
            };
        }
        
        float normalized = Mathf.Repeat(Mathf.Repeat(angle, 360) + _angleSectorValue, 360f);
        int direction = Mathf.FloorToInt(normalized / 90f);
        return direction switch
        {
            0 => _rightAttack.position,
            1 => _upAttack.position,
            2 => _leftAttack.position,
            3 => _downAttack.position,
            _ => Vector3.zero
        };
    }

    private void PreformAttack(Vector3 offset)
    {
        _player.TriggerAttack();
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
    IEnumerator AttackRoutine(Vector3 attackPos)
    {
        GameObject hitbox = Instantiate(hitboxPrefab, 
            attackPos, 
            Quaternion.identity, 
            transform);
        
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();

        hitboxScript.thisPlayer = _player;
        
        hitboxScript.SetAttackDamage(10);
        
        // ChangeToCombatSprite(); // <- change once art is in
        
        yield return new WaitForSeconds(0.2f);
        
        //check to see if the hit was successful to increment ultimate attack meter
        if (hitboxScript.IsSuccessfulHit())
        {
            OnSuccessfulHit?.Invoke();
            hitboxScript.ResetSuccessfulHit();
        }
        //TODO: adjust time once done with testing, 2 seconds is too long for a real hitbox
        Destroy(hitbox, _hitboxLifetime);
        // ResetSprite();
    }

    IEnumerator UltimateAttackRoutine()
    {
        GameObject ultimateHitbox = Instantiate(ultimateHitboxPrefab, 
            transform.position, 
            Quaternion.identity, 
            transform);
        ultimateHitbox.GetComponent<UltimateAttackHitbox>().thisPlayer = _player;
        
        // ChangeToCombatSprite();
        yield return new WaitForSeconds(0.2f);
        Destroy(ultimateHitbox, 2f);
        // ResetSprite();
        
    }

    //I know this is mostly repeat code, but the animations will need to be different than the primary attack
    //so it will be useful to have its own coroutine
    IEnumerator SecondaryAttackRoutine(Vector3 attackPos)
    {
        GameObject hitbox = Instantiate(hitboxPrefab, 
            attackPos, 
            Quaternion.identity, 
            transform);
        
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();

        hitboxScript.thisPlayer = _player;
        hitboxScript.SetAttackDamage(25);
        
        // ChangeToCombatSprite();
        
        yield return new WaitForSeconds(0.2f);
        
        //check to see if the hit was successful to increment ultimate attack meter
        if (hitboxScript.IsSuccessfulHit())
        {
            OnSuccessfulHit?.Invoke();
            hitboxScript.ResetSuccessfulHit();
        }
        //TODO: adjust time once done with testing, 2 seconds is too long for a real hitbox
        Destroy(hitbox, _hitboxLifetime);
        // ResetSprite();
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