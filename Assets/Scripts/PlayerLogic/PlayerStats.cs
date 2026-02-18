using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "PlayerStats", order = 2, fileName = "New Player Stats")]
public class PlayerStats : ScriptableObject
{
    public WeightClass WeightClass;
}
