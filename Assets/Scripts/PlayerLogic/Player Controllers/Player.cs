using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : PlayerBase
{
    [SerializeField] private PlayerStats _defaultStats;
 
    public event Action<Player> OnDeath;
    public event Action<float> UICallback_PlayerHealthChange;
    
    // ===== References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats playerStats { private set; get; }
    public UltimateAttackTracker ultimateAttackTracker { private set; get; }
    public float PlayerHealth
    {
        get
        {
            if (hurtbox != null) return hurtbox.HP;
            return 0;
        }
        set 
        {
            if (hurtbox != null)
            {
                hurtbox.HP = value;
                UICallback_PlayerHealthChange?.Invoke(hurtbox.HP);
            }
        }
    }
    
    public bool IsFrozen { get; private set; }
    public float Movement => _playerInputManager.GetMoveInput().x;
    public bool InAir => !_jumpModule.Grounded;
    public void TriggerAttack(int direction) { _animation.TriggerAttack(direction); }
    public void TriggerUltimate() {_animation.TriggerUltimate();}
    public void TriggerJump() { _animation.TriggerJump(); }

    // ===== Internal References =====
    private PlayerStateMachine  _playerStateMachine;
    private PlayerAnimation _animation;
    private Powerup _attachedPowerup;
    private float _savedMovementSpeed;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private PlatformerJumpModule _jumpModule;

    private Coroutine _hitStunCoroutine;
    private Coroutine _damageCoroutine;
    private Coroutine _freezeMovementCoroutine;
    
    new void Awake()
    {
        base.Awake();
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponentInChildren<PlayerAnimation>();
        _playerStateMachine = GetComponent<PlayerStateMachine>();
        ultimateAttackTracker = GetComponent<UltimateAttackTracker>();
        _jumpModule = GetComponent<PlatformerJumpModule>();
        _horizontalMovementModule = GetComponent<PlatformerHorizontalMovementModule>();
        ResetWeightClass();

        _jumpModule.JumpTriggered += TriggerJump;
    }
    
    private void OnDestroy()
    {
        OnDeath = null;
        if (_jumpModule) _jumpModule.JumpTriggered -= TriggerJump;
    }

    public void Register(Action<Player> deathCallback)
    {
        base.Register();
        OnDeath = deathCallback;
    } 

    #region Inputs

    public void UsePower(InputAction.CallbackContext context)
    {
        if (!_playerInputManager.isInputEnabled) return;
        if (context.performed) powerups.UsePower();
    }

    #endregion
    
    #region Health
    public void TakeDamage(float damage)
    {
        PlayerHealth -= damage;
        _animation.DamageFlash();
        
        if (PlayerHealth <= 0)
        {
            OnDeath(this);
        }
    }

    public void InstaDeath()
    {
        OnDeath(this);
    }

    public void HealHP(int heal)
    {
        PlayerHealth += heal;
        if (PlayerHealth > 200)
        {
            PlayerHealth = 200;
        }
    }

    public void ResetHealth()
    {
        hurtbox.ResetHealth();
    }
    #endregion
    
    #region Combat 
    public void ApplyHitStun(float duration)
    { 
        //if null starts hitstun, else do nothing which is what we want 
        _hitStunCoroutine ??= StartCoroutine(AddHitStun(duration));
    }
    
    public void ApplyKnockback(Vector2 direction, float knockbackForce)
    {
        _damageCoroutine ??= StartCoroutine(AddKnockback(direction, knockbackForce));
    }
    
    IEnumerator AddKnockback(Vector2 direction, float knockbackForce)
    {
        float elapsedTime = 0f;
        while (elapsedTime < CombatParameters.knockbackDuration)
        {
            float normalizedTime = elapsedTime / CombatParameters.knockbackDuration;
            float currentForce = CombatParameters.knockbackCurve.Evaluate(normalizedTime) * knockbackForce;

            _rigidbody2D.AddForce(direction * currentForce * 10);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _damageCoroutine = null;
    }

    IEnumerator AddHitStun(float duration)
    {
        _playerStateMachine.EnterHitStun();
        FreezePlayerMovement();
        yield return new WaitForSeconds(duration);
        UnfreezePlayerMovement();
        _playerStateMachine.ResetState();
        _hitStunCoroutine = null;
    }

    public void ResetWeightClass()
    {
        if (_defaultStats == null) Debug.LogError("Default Stats should not be null!");
        AssignWeightClass(_defaultStats);
    }
    
    public void AssignWeightClass(PlayerStats stats)
    {
        playerStats = stats;
        switch (stats.Type)
        {
            case WeightClassType.Light:
                _jumpModule.Config.SetJumpTypeToLight();
                _horizontalMovementModule.SetMovementTypeToFast();
                break;
            case WeightClassType.Heavy:
                _jumpModule.Config.SetJumpTypeToHeavy();
                _horizontalMovementModule.SetMovementTypeToSlow();
                break;
            case WeightClassType.Default:
                //reset to regular stats if not light or heavy
                _jumpModule.Config.ResetJumpType();
                _horizontalMovementModule.ResetMovement();
                break;
            case WeightClassType.Custom:
                _jumpModule.Config.SetParameters(stats.PlayerParameters);
                _horizontalMovementModule.SetParameters(stats.PlayerParameters);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    #endregion
    
    public void FreezePlayerMovement()
    {
        IsFrozen = true;
        _savedMovementSpeed = _horizontalMovementModule.GetMovementSpeed();
        _horizontalMovementModule.SetMovementSpeed(0f);
        _jumpModule.Config.DisableJump();
    }

    public void UnfreezePlayerMovement()
    {
        IsFrozen = false;
        _horizontalMovementModule.SetMovementSpeed(_savedMovementSpeed);
        _jumpModule.Config.ReEnableJump();
    }

    /// <summary>
    /// Freeze the player for a brief moment 
    /// </summary>
    public void FreezePlayer(float _duration)
    {
        _freezeMovementCoroutine ??= StartCoroutine(Freeze());
        return;
        
        IEnumerator Freeze()
        {
            if (IsFrozen) yield break;
        
            FreezePlayerMovement();
            yield return new WaitForSeconds(_duration);
            UnfreezePlayerMovement();
            yield return new WaitForSeconds(0.35f);
            _freezeMovementCoroutine = null;
        }
    }
}

