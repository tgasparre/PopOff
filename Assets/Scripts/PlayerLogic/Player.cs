using System;
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
    private PlayerAnimation _animation;
    private Powerup attachedPowerup;
    private float movementSpeed;
    private UltimateAttackTracker _ultimateAttackTracker;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private FighterController _fighterController;
    private Rigidbody2D _rigidbody2D;

    private PlayerInput _playerInput;
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
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponent<PlayerAnimation>();
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
    
    public void TakeDamage(int damage)
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


