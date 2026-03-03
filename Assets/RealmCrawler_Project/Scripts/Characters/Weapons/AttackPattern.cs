using System;
using UnityEngine;

/// <summary>
/// Data structure for attack timing, damage, and hitbox configuration
/// </summary>
[Serializable]
public class AttackPattern
{
    [Header("Timing")]
    [Tooltip("Time spent in windup phase (preparation)")]
    public float windupTime = 0.1f;
    
    [Tooltip("Time spent in execute phase (active hit detection)")]
    public float executeTime = 0.1f;
    
    [Tooltip("Time spent in recovery phase (can't interrupt)")]
    public float recoveryTime = 0.2f;
    
    [Header("Combat")]
    [Tooltip("Base damage dealt by this attack")]
    public float damage = 10f;
    
    [Tooltip("Whether this attack can chain into another attack")]
    public bool canChain = false;
    
    [Header("Hitbox")]
    [Tooltip("Range of the hitbox from weapon pivot")]
    public float hitboxRange = 1.5f;
    
    [Tooltip("Angle of the hitbox in degrees")]
    public float hitboxAngle = 45f;
    
    [Tooltip("Height of the hitbox")]
    public float hitboxHeight = 1f;
    
    [Header("Effects")]
    [Tooltip("Knockback force applied to enemies")]
    public float knockbackForce = 5f;
    
    [Tooltip("Whether to spawn hit effect on impact")]
    public bool spawnHitEffect = true;
}
