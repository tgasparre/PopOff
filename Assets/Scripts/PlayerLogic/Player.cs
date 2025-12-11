using System;
using ControllerSystem.Platformer2D;
using InputManagement;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats playerStatsTemplate;
    public PlayerStats playerStats;
    public AttackHurtbox hurtbox;
    private float movementSpeed;
    public event Action PlayerDied;

    void Awake()
    {
        playerStats = Instantiate(playerStatsTemplate);
        hurtbox = GetComponentInChildren<AttackHurtbox>();
    }

    void Update()
    {
        //if a player is offscreen, kill them (like in smash)
        if (SpriteTools.IsOffScreen(gameObject.GetComponentInChildren<SpriteRenderer>()))
        {
            KillPlayer();
        }
    }

    public void KillPlayer()
    {
        gameObject.GetComponent<UltimateAttackTracker>().playerUI.DeletePlayerUI();
        Destroy(this.GameObject());
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
        movementSpeed = gameObject.GetComponent<PlatformerHorizontalMovementModule>().GetMovementSpeed();
        gameObject.GetComponent<PlatformerHorizontalMovementModule>().SetMovementSpeed(0f);
    }

    public void UnfreezePlayerMovement()
    {
        gameObject.GetComponent<PlatformerHorizontalMovementModule>().SetMovementSpeed(movementSpeed);
    }

    public void SetInputManager(InputManager inputManager)
    {
        gameObject.GetComponent<FighterController>().InputManager = inputManager;
    }

    void OnDestroy()
    {
        Destroy(playerStats);
    }
    
}

