using System;
using UnityEngine;

public class AttackHurtbox : MonoBehaviour
{
    public Player player;
    [SerializeField] private float _startingHealth = 200;
    public float HP { get; set; }

    private void Awake()
    {
        ResetHealth();
    }

    public void TakeDamage(float damage)
    {
        if (player != null) 
            player.TakeDamage(damage);
    }

    public void ResetHealth()
    {
        HP = _startingHealth;
    }

    public void InstantDeath()
    {
        player.InstaDeath();
    }
    
}
