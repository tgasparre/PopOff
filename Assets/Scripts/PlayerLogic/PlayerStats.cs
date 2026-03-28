using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "PlayerStats", order = 2, fileName = "New Player Stats")]
public class PlayerStats : ScriptableObject
{
    [SerializeField] [HideInInspector] private WeightClassType _type = WeightClassType.Default;
    public WeightClassType Type => _type;

    [SerializeField] [HideInInspector] private float _knockbackMultiplier = WeightParameters.regularKnockbackMultiplier;
    [SerializeField] [HideInInspector] private float _damageMultiplier = WeightParameters.regularDamageMultiplier;

    [SerializeField] private CustomPlayerParameters _parameters;
    public CustomPlayerParameters PlayerParameters => _parameters;

    public float KnockbackMultiplier()
    {
        return _type switch
        {
            WeightClassType.Default => WeightParameters.regularKnockbackMultiplier,
            WeightClassType.Light => WeightParameters.lightKnockbackMultiplier,
            WeightClassType.Heavy => WeightParameters.heavyKnockbackMultiplier,
            WeightClassType.Custom => _knockbackMultiplier,
            _ => 1
        };
    }
    
    public float DamageMultiplier()
    {
        return _type switch
        {
            WeightClassType.Default => WeightParameters.regularDamageMultiplier,
            WeightClassType.Light => WeightParameters.lightDamageMultiplier,
            WeightClassType.Heavy => WeightParameters.heavyDamageMultiplier,
            WeightClassType.Custom => _damageMultiplier,
            _ => 1
        };
    }
}

public enum WeightClassType
{
    Light,
    Default,
    Heavy,
    Custom
}

#if UNITY_EDITOR

[CustomEditor(typeof(PlayerStats))]
public class PlayerStatsEditor : Editor
{
    private SerializedProperty weightClassType;
    private SerializedProperty parameters;
    private SerializedProperty knockback;
    private SerializedProperty damage;
    
    private void OnEnable()
    {
        weightClassType = serializedObject.FindProperty("_type");
        parameters = serializedObject.FindProperty("_parameters");
        knockback = serializedObject.FindProperty("_knockbackMultiplier");
        damage = serializedObject.FindProperty("_damageMultiplier");
    }
    public override void OnInspectorGUI()
    {
        GUI.enabled = false;
        EditorGUILayout.ObjectField("Script:", MonoScript.FromScriptableObject((PlayerStats)target), typeof(PlayerStats), false);
        GUI.enabled = true;
        
        PlayerStats stats = target as PlayerStats;
        
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(weightClassType);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
        
        if (stats.Type == WeightClassType.Custom)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(knockback);
            EditorGUILayout.PropertyField(damage);
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(parameters, true);
        } 
    }

    private void ResetCustom(SerializedProperty element)
    {
        SerializedProperty turnAroundSpeed = element.FindPropertyRelative("turnAroundSpeed");
        SerializedProperty groundAcceleration = element.FindPropertyRelative("groundAcceleration");
        SerializedProperty airAcceleration = element.FindPropertyRelative("airAcceleration");
        SerializedProperty groundSpeed = element.FindPropertyRelative("groundSpeed");
        SerializedProperty airSpeed = element.FindPropertyRelative("airSpeed");
        SerializedProperty groundDrag = element.FindPropertyRelative("groundDrag");
        SerializedProperty airDrag = element.FindPropertyRelative("airDrag");
        
        SerializedProperty numDoubleJump = element.FindPropertyRelative("numDoubleJump");
        SerializedProperty jumpHeight = element.FindPropertyRelative("jumpHeight");
        SerializedProperty jumpEndEarlyForce = element.FindPropertyRelative("jumpEndEarlyForce");

        turnAroundSpeed.floatValue = MovementParameters.turnAroundSpeedMultiplier;
        groundAcceleration.floatValue = MovementParameters.groundAccelerationTime;
        airAcceleration.floatValue = MovementParameters.airAccelerationTime;
        groundSpeed.floatValue = MovementParameters.groundSpeed;
        airSpeed.floatValue = MovementParameters.airSpeed;
        groundDrag.floatValue = MovementParameters.groundDrag;
        airDrag.floatValue = MovementParameters.airDrag;

        numDoubleJump.intValue = MovementParameters.defaultDoubleJumps;
        jumpHeight.floatValue = MovementParameters.defaultJumpHeight;
        jumpEndEarlyForce.floatValue = MovementParameters.defaultJumpEndEarlyForce;

    }
}

#endif