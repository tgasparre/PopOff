using System;
using ControllerSystem.Platformer2D;
using InputManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerStats playerStatsTemplate;
    
    // ===== HEADER: References =====
    public PlayerPowerups powerups { private set; get; }
    public AttackHurtbox hurtbox { private set; get; }
    public PlayerStats playerStats { private set; get; }
    
    public event Action PlayerDied;
    
    // ===== HEADER: Internal References =====
    private PlayerAnimation _animation;
    private Powerup attachedPowerup; 
    private float movementSpeed;
    private UltimateAttackTracker _ultimateAttackTracker;
    private PlatformerHorizontalMovementModule _horizontalMovementModule;
    private FighterController _fighterController;

    void Awake()
    {
        playerStats = Instantiate(playerStatsTemplate);
        hurtbox = GetComponentInChildren<AttackHurtbox>();
        powerups = GetComponent<PlayerPowerups>();
        _animation = GetComponent<PlayerAnimation>();
        _ultimateAttackTracker = GetComponent<UltimateAttackTracker>();
        _fighterController = GetComponent<FighterController>();
        _horizontalMovementModule = GetComponent<PlatformerHorizontalMovementModule>();
    }

    void Update()
    {
        //TODO change so there is a buffer zone, so they don't die right away 
        //if a player is offscreen, kill them (like in smash)
        if (SpriteTools.IsOffScreen(GetComponentInChildren<SpriteRenderer>()))
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        _ultimateAttackTracker.playerUI.DeletePlayerUI();
        Destroy(gameObject);
        PlayerDied?.Invoke();
    }
    
    public void TakeDamage(int damage)
    {
        hurtbox.HP -= damage;
        if (hurtbox.HP <= 0)
        {
            KillPlayer();
        }
        Debug.Log("TakeDamage was called");
    }
    
    public void HealHP(int heal)
    {
        hurtbox.HP += heal;
        if (hurtbox.HP > 200)
        {
            hurtbox.HP = 200;
        }
    }
    
    public void FreezePlayerMovement()
    {
        movementSpeed = _horizontalMovementModule.GetMovementSpeed();
        _horizontalMovementModule.SetMovementSpeed(0f);
    }

    public void UnfreezePlayerMovement()
    {
        _horizontalMovementModule.SetMovementSpeed(movementSpeed);
    }
    
    #region Powerup
    public void UsePower(InputAction.CallbackContext context)
    {
        powerups.UsePower();
    }
    #endregion
    
    public void SetInputManager(InputManager inputManager)
    {
        _fighterController.InputManager = inputManager;
    }

    public bool IsFacingLeft => _fighterController.FacingLeft;
    public int FacingLeftValue => IsFacingLeft ? -1 : 1;
    
    void OnDestroy()
    {
        Destroy(playerStats);
    }
}

