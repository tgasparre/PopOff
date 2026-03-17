using System;
using System.Collections;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatInputHandler : MonoBehaviour
{
    public InputManager InputManager;
    public GameObject hitboxPrefab;
    public GameObject ultimateHitboxPrefab;

    private Vector2 moveInput;
    [Header("Attack Settings")]
    [SerializeField] private float _hitboxLifetime = 0.5f;
    [SerializeField] private float _ultimateHitboxLifetime = 0.8f;
    [Header("Attack Locations")]
    [SerializeField] [Range(0,90)] private float _angleSectorValue = 45;
    [SerializeField] private Transform _upAttack;
    [SerializeField] private Transform _downAttack;
    [SerializeField] private Transform _leftAttack;
    [SerializeField] private Transform _rightAttack;
    
    private bool UltimateAttackEnabled = false; 
    private UltimateAttackTracker _tracker;

    private Player _player;

    private void Awake()
    {
        _player = GetComponent<Player>();
        _tracker = GetComponent<UltimateAttackTracker>();
        _tracker.OnUltimateAttackUnlocked += OnUltimateAttackUnlocked;
    }
    private void OnDestroy()
    {
        _tracker.OnUltimateAttackUnlocked -= OnUltimateAttackUnlocked;
    }
    
    #region Inputs

    public void OnUltimateAttack(InputAction.CallbackContext context)
    {
        if (!InputManager.isInputEnabled) return;
        if (UltimateAttackEnabled)
        {
            Vector3 attackDirection = GetAttackDirection(true);
            PreformUltimate(attackDirection);
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
            PreformAttack(attackDirection);
        }
    }
    
    #endregion
    
    private Vector3 GetAttackDirection(bool limitVertical = false)
    {
        moveInput = InputManager.GetMoveInput();
        float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
        if (angle == 0 || limitVertical)
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

    private void PreformAttack(Vector2 offset)
    {
        _player.TriggerAttack();
        GameObject hitbox = Instantiate(hitboxPrefab, offset, Quaternion.identity, transform);
        AttackHitbox hitboxScript = hitbox.GetComponent<AttackHitbox>();
        
        hitboxScript.SpawnHitbox(_player, AttackHitbox.HitboxType.Regular, () =>
        {
            _tracker.OnSuccessfulHit();
        });
        
        Destroy(hitbox, _hitboxLifetime);
    }

    private void PreformUltimate(Vector2 offset)
    {
        _player.TriggerUltimate();
        GameObject ultimateHitbox = Instantiate(ultimateHitboxPrefab, offset, Quaternion.identity, transform);
        AttackHitbox hitboxScript = ultimateHitbox.GetComponent<AttackHitbox>();

        hitboxScript.SpawnHitbox(_player, AttackHitbox.HitboxType.Ultimate, null);

        Destroy(ultimateHitbox, _ultimateHitboxLifetime);
    }
    
    private void OnUltimateAttackUnlocked()
    {
        //TODO play sound
        UltimateAttackEnabled = true;
    }

    private void ResetUltimateAttack()
    {
        UltimateAttackEnabled = false;
        _tracker.ResetTracker();
    }
    
    IEnumerator AttackRoutine(Vector3 attackPos)
    {
       
        yield break;
    }

}