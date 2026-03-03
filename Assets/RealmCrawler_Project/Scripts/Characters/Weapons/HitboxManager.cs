using UnityEngine;

/// <summary>
/// Manages melee hitboxes for weapon attacks.
/// Spawns and destroys hitboxes during attack Execute phase.
/// </summary>
public class HitboxManager : MonoBehaviour
{
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private Transform _hitboxOrigin;
    
    private Collider2D _activeHitbox;
    private AttackPattern _currentPattern;
    private float _hitboxLifetime;
    
    public void SpawnHitbox(AttackPattern pattern, Vector3 position, Quaternion rotation)
    {
        if (_activeHitbox != null)
        {
            Destroy(_activeHitbox.gameObject);
        }
        
        _currentPattern = pattern;
        _hitboxLifetime = pattern.executeTime;
        
        // Create hitbox GameObject
        GameObject hitboxObj = new GameObject($"Hitbox_{pattern.attackType}");
        hitboxObj.transform.SetParent(_hitboxOrigin);
        hitboxObj.transform.position = position;
        hitboxObj.transform.rotation = rotation;
        
        // Create BoxCollider2D based on weapon range and angle
        BoxCollider2D boxCollider = hitboxObj.AddComponent<BoxCollider2D>();
        boxCollider.size = new Vector2(pattern.hitboxRange, pattern.hitboxRange * Mathf.Tan(pattern.hitboxAngle * 0.5f));
        boxCollider.offset = new Vector2(pattern.hitboxRange * 0.5f, 0);
        boxCollider.isTrigger = true;
        
        _activeHitbox = boxCollider;
        
        // Destroy hitbox after execute phase
        Destroy(hitboxObj, _hitboxLifetime);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_activeHitbox == null) return;
        
        if ((1 << other.gameObject.layer) == _enemyLayer)
        {
            // Apply damage to enemy
            ApplyDamage(other.gameObject);
        }
    }
    
    private void ApplyDamage(GameObject target)
    {
        // TODO: Connect to enemy health system
        Debug.Log($"Hit {target.name} for {_currentPattern.damage} damage");
    }
}
