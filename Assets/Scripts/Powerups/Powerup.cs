using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


public abstract class Powerup : ScriptableObject
{
    [Header("Basic Settings")]
    [SerializeField] private string _name = "default powerup"; 
    [SerializeField] private PowerupType type;
    [SerializeField] [Tooltip("times power up can be used")] protected float _charges = 3f;
    [SerializeField] protected float _useCooldown = 0.5f;

    [Header("Visual Settings")]
    [SerializeField] protected Sprite _icon;
    
    public string Name => _name;
    public float UseCooldown => _useCooldown;

    public abstract void UsePowerup(PlayerPowerups powerups);
    public abstract void Expire(PlayerPowerups powerups);
    
    public Sprite GetIcon() { return _icon; }
    public float GetCharges() { return _charges; }
    public bool HasTimer => type == PowerupType.Timer;

    public enum PowerupType
    {
        Timer,
        Charge
    }
}

[System.Serializable]
public class PowerupStats
{
    public float speed = 1f;
    public float size = 1f;
    
    [Header("Damage Settings")]
    public PowerupType type;
    public int damage = 20;
    public float glueDuration = 1f;
    
    public enum PowerupType
    {
        Damage,
        Glue
    } 
}

