using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats", order = 2, fileName = "New Player Stats")]
public class PlayerStats : ScriptableObject
{
    public int HP;
    public string PlayerName;
    public float DamageMultiplier = 1;
    public Vector3 LastPosition;
}
