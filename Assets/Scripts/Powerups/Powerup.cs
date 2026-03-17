using System;
using UnityEngine;


public abstract class Powerup : ScriptableObject
{
    [Header("Basic Settings")]
    [SerializeField] private new string _name = "default powerup"; 
    [SerializeField] [Tooltip("time powerup is usable, -1 for single use")] protected float _timeToExpire = -1f;
    [SerializeField] protected float _useCooldown = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] protected Sprite _icon;

    public string Name => _name;

    public abstract void UsePowerup(PlayerPowerups powerups);
    public abstract void Expire(PlayerPowerups powerups);
    
    public Sprite GetIcon() { return _icon; }
    public float GetExpireTime() {return _timeToExpire;}
    public bool IsInfinite => _timeToExpire == -1;
}

[System.Serializable]
public class PowerupStats
{
    public float speed = 1f;
    public float size = 1f;
    public int damage;
}


