using System;
using ControllerSystem.Platformer2D;
using InputManagement;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerStats playerStats;
    public event Action PlayerDied;

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
        Destroy(this.GameObject());
        PlayerDied?.Invoke();
    }
    
    public void TakeDamage(int damage)
    {
        playerStats.HP -= damage;
        if (playerStats.HP <= 0)
        {
            KillPlayer();
        }
    }
    
    public void HealHP(int heal)
    {
        playerStats.HP += heal;
        if (playerStats.HP > 200)
        {
            playerStats.HP = 200;
        }
    }
    
    public void FreezePlayerMovement()
    {
        // make the player object stay at its current coordinates
        // ensure that they don't fall out of the air (freeze gravity?)
        // block input if needed
        this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void UnfreezePlayerMovement()
    {
        this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        this.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void SetInputManager(InputManager inputManager)
    {
        gameObject.GetComponent<FighterController>().InputManager = inputManager;
    }
    
    
}

