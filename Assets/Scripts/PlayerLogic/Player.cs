using System;
using System.Collections;
using ControllerSystem.Platformer2D;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerStats playerStatsTemplate;

    // ===== References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats playerStats { private set; get; }

    public bool IsFacingLeft => _fighterController.FacingLeft;
    public int FacingLeftValue => IsFacingLeft ? -1 : 1;

    // ===== Internal References =====
    private PlayerStateMachine  playerStateMachine;
    private PlayerAnimation _animation;
    private Powerup attachedPowerup;
    private float movementSpeed;
    private UltimateAttackTracker _ultimateAttackTracker;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private FighterController _fighterController;
    private Rigidbody2D _rigidbody2D;

    private PlayerInput _playerInput;
    private Coroutine hitStunCoroutine;
    public void Register(PlayerInput input)
    {
        _playerInput = input;
        ActivePlayersTracker.LookForPlayerSpawn(this);
        DontDestroyOnLoad(gameObject);
    }
    public int PlayerIndex => _playerInput.playerIndex;


    void Awake()
    {
        playerStats = Instantiate(playerStatsTemplate);
        AssignWeightClass("regular");
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponent<PlayerAnimation>();
        playerStateMachine = GetComponent<PlayerStateMachine>();
        _ultimateAttackTracker = GetComponent<UltimateAttackTracker>();
        _fighterController = GetComponent<FighterController>();
        _horizontalMovementModule = GetComponent<PlatformerHorizontalMovementModule>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
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

    public void ApplyHitStun(float duration)
    {
        if (hitStunCoroutine != null)
            StopCoroutine(hitStunCoroutine);
        hitStunCoroutine = StartCoroutine(AddHitStun(duration));
    }


    public void ApplyKnockback(Vector2 direction, float knockbackMultiplier, float knockbackForce)
    {
        StartCoroutine(AddKnockback(direction, knockbackMultiplier, knockbackForce));
    }

    //may need testing to ensure movement feels good
    public void AssignWeightClass(String wClass)
    {
        playerStats.WeightClass.ChangeWeightClass(wClass);
        PlatformerJumpModule jumpModule = gameObject.GetComponent<PlatformerJumpModule>();
        PlatformerHorizontalMovementModule movementModule = gameObject.GetComponent<PlatformerHorizontalMovementModule>();
        if (wClass == "light")
        {
            jumpModule.Config.SetJumpTypeToLight();
            movementModule.SetMovementTypeToFast();
        }
        else if (wClass == "heavy")
        {
            jumpModule.Config.SetJumpTypeToHeavy();
            movementModule.SetMovementTypeToSlow();
        }
        else
        {
            //reset to regular stats if not light or heavy
            jumpModule.Config.ResetJumpType();
            movementModule.ResetMovement();
        }
    }
    
    public void FreezePlayerMovement()
    {
        movementSpeed = gameObject.GetComponent<PlatformerHorizontalMovementModule>().GetMovementSpeed();
        gameObject.GetComponent<PlatformerHorizontalMovementModule>().SetMovementSpeed(0f);
    }

    public void UnfreezePlayerMovement()
    {
        gameObject.GetComponent<PlatformerHorizontalMovementModule>().SetMovementSpeed(movementSpeed);
    }

    public void FreezePlayer()
    {
        _rigidbody2D.bodyType = RigidbodyType2D.Static;
    }
    
    IEnumerator AddKnockback(Vector2 direction, float knockbackMultiplier, float knockbackForce)
    {
        Rigidbody2D rb = gameObject.GetComponent<Rigidbody2D>();
        float elapsedTime = 0f;
            
        while (elapsedTime < CombatParameters.knockbackDuration)
        {
            float normalizedTime = elapsedTime / CombatParameters.knockbackDuration;
            float currentForce = CombatParameters.knockbackCurve.Evaluate(normalizedTime) 
                                     * (knockbackForce * knockbackMultiplier);
            
            rb.linearVelocity = direction * currentForce;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        rb.linearVelocity = Vector2.zero;
    }

    IEnumerator AddHitStun(float duration)
    {
        playerStateMachine.EnterHitStun();
        FreezePlayerMovement();
        yield return new WaitForSeconds(duration);
        UnfreezePlayerMovement();
        playerStateMachine.ResetState();
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


