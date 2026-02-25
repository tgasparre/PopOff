using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerStats playerStatsTemplate;

    // ===== References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats playerStats { private set; get; }

    public bool IsFacingLeft => FacingLeftValue == -1;
    public int FacingLeftValue => _playerInputManager.GetFacingDirection();

    public float Movement => _playerInputManager.GetMoveInput().x;
    public bool InAir => Mathf.Abs(_rigidbody2D.linearVelocityY) > 0.5;
    public void TriggerAttack() { _animation.TriggerAttack(); }

    // ===== Internal References =====
    private PlayerStateMachine  playerStateMachine;
    private PlayerAnimation _animation;
    private Powerup attachedPowerup;
    private float movementSpeed;
    private UltimateAttackTracker _ultimateAttackTracker;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private PlatformerJumpModule _jumpModule;
    private FighterController _fighterController;
    private InputManager _playerInputManager;
    private Rigidbody2D _rigidbody2D;

    private PlayerInput _playerInput;
    public void Register(PlayerInput input)
    {
        _playerInput = input;
        ActivePlayersTracker.LookForPlayerSpawn(this);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;

    private Coroutine _hitStunCoroutine;
    private Coroutine _damageCoroutine;
    
    void Awake()
    {
        playerStats = Instantiate(playerStatsTemplate);
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponentInChildren<PlayerAnimation>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        _ultimateAttackTracker = GetComponent<UltimateAttackTracker>();
        _fighterController = GetComponent<FighterController>();
        _jumpModule = GetComponent<PlatformerJumpModule>();
        _horizontalMovementModule = GetComponent<PlatformerHorizontalMovementModule>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerInputManager = GetComponent<InputManager>();
        AssignWeightClass();
    }

    #region Inputs

    public void UsePower(InputAction.CallbackContext context)
    {
        if (context.performed) powerups.UsePower();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        if (context.performed) Game.currentState = GameStates.Pause;
    }

    #endregion
    
    public void TakeDamage(float damage)
    {
        hurtbox.HP -= damage;
        _animation.DamageFlash();
        
        if (hurtbox.HP <= 0)
        {
            Game.Instance.OnPlayerDied(this);
        }
    }

    public void HealHP(int heal)
    {
        hurtbox.HP += heal;
        if (hurtbox.HP > 200)
        {
            hurtbox.HP = 200;
        }
    }

    public void ResetHealth()
    {
        hurtbox.ResetHealth();
    }

    public void ApplyHitStun(float duration)
    { 
        //if null starts hitstun, else do nothing which is what we want 
        _hitStunCoroutine ??= StartCoroutine(AddHitStun(duration));
    }
    
    public void ApplyKnockback(Vector2 direction, float knockbackMultiplier, float knockbackForce)
    {
        _damageCoroutine ??= StartCoroutine(AddKnockback(direction, knockbackMultiplier, knockbackForce));
    }
    
    IEnumerator AddKnockback(Vector2 direction, float knockbackMultiplier, float knockbackForce)
    {
        float elapsedTime = 0f;
        while (elapsedTime < CombatParameters.knockbackDuration)
        {
            float normalizedTime = elapsedTime / CombatParameters.knockbackDuration;
            float currentForce = CombatParameters.knockbackCurve.Evaluate(normalizedTime) * (knockbackForce * knockbackMultiplier);

            _rigidbody2D.AddForce(direction * currentForce * 10);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _damageCoroutine = null;
    }

    IEnumerator AddHitStun(float duration)
    {
        playerStateMachine.EnterHitStun();
        FreezePlayerMovement();
        yield return new WaitForSeconds(duration);
        UnfreezePlayerMovement();
        playerStateMachine.ResetState();
        _hitStunCoroutine = null;
    }

    //may need testing to ensure movement feels good
    public void AssignWeightClass(WeightClassType wClass = WeightClassType.Default)
    {
        playerStats.WeightClass.ChangeWeightClass(wClass);
        //PlatformerJumpModule jumpModule = gameObject.GetComponent<PlatformerJumpModule>();
        //PlatformerHorizontalMovementModule movementModule = gameObject.GetComponent<PlatformerHorizontalMovementModule>();
        if (wClass == WeightClassType.Light)
        {
            _jumpModule.Config.SetJumpTypeToLight();
            _horizontalMovementModule.SetMovementTypeToFast();
        }
        else if (wClass == WeightClassType.Heavy)
        {
            _jumpModule.Config.SetJumpTypeToHeavy();
            _horizontalMovementModule.SetMovementTypeToSlow();
        }
        else
        {
            //reset to regular stats if not light or heavy
            _jumpModule.Config.ResetJumpType();
            _horizontalMovementModule.ResetMovement();
        }
    }
    
    public void FreezePlayerMovement()
    {
        movementSpeed = _horizontalMovementModule.GetMovementSpeed();
        _horizontalMovementModule.SetMovementSpeed(0f);
        _jumpModule.Config.DisableJump();
    }

    public void UnfreezePlayerMovement()
    {
        _horizontalMovementModule.SetMovementSpeed(movementSpeed);
        _jumpModule.Config.ReEnableJump();
    }

    public void FreezePlayer()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
    }

    public void UnfreezePlayer()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }
}


// void Update()
// {
//     //TODO change so there is a buffer zone, so they don't die right away 
//     //if a player is offscreen, kill them (like in smash)
//     if (SpriteTools.IsOffScreen(GetComponentInChildren<SpriteRenderer>()))
//     {
//         KillPlayer();
//     }
// }

// public void KillPlayer()
// {
//     _ultimateAttackTracker.playerUI.DeletePlayerUI();
//     Destroy(gameObject);
//     PlayerDied?.Invoke();
// }
//     
// public void TakeDamage(int damage)
// {
//     hurtbox.HP -= damage;
//     if (hurtbox.HP <= 0)
//     {
//         KillPlayer();
//     }
//     Debug.Log("TakeDamage was called");
// }
//     
// public void HealHP(int heal)
// {
//     hurtbox.HP += heal;
//     if (hurtbox.HP > 200)
//     {
//         hurtbox.HP = 200;
//     }
// }
//     
// public void FreezePlayerMovement()
// {
//     movementSpeed = _horizontalMovementModule.GetMovementSpeed();
//     _horizontalMovementModule.SetMovementSpeed(0f);
// }
//
// public void UnfreezePlayerMovement()
// {
//     _horizontalMovementModule.SetMovementSpeed(movementSpeed);
// }
// void OnDestroy()
// {
//     Destroy(playerStats);
// }
// public void SetInputManager(InputManager inputManager)
// {
//     _fighterController.InputManager = inputManager;
// }


