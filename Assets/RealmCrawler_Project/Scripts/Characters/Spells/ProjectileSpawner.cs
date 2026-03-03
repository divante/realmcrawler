using UnityEngine;

/// <summary>
/// Spawns and manages projectiles for spell effects.
/// Handles projectile lifecycle, movement, and collision detection.
/// </summary>
public class ProjectileSpawner : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform spawnOrigin;
    [SerializeField] private float spawnOffset = 0.5f;
    
    [Header("Projectile Behavior")]
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float projectileLifetime = 3f;
    [SerializeField] private float damage = 10f;
    [SerializeField] private LayerMask targetLayers;
    
    private GameObject[] activeProjectiles = new GameObject[10];
    private int activeProjectileCount = 0;

    private void Awake()
    {
        if (spawnOrigin == null)
            spawnOrigin = transform;
    }

    /// <summary>
    /// Spawn a projectile at the given position with direction.
    /// </summary>
    public void SpawnProjectile(Vector3 position, Vector3 direction, float damageOverride = -1f)
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("[ProjectileSpawner] No projectile prefab assigned!");
            return;
        }

        if (activeProjectileCount >= activeProjectiles.Length)
        {
            Debug.LogWarning("[ProjectileSpawner] Max active projectiles reached!");
            return;
        }

        float finalDamage = damageOverride > 0 ? damageOverride : damage;
        
        // Calculate spawn position with offset
        Vector3 spawnPosition = position + direction.normalized * spawnOffset;
        
        // Spawn projectile
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        ProjectileData projectileData = projectile.GetComponent<ProjectileData>();
        
        if (projectileData != null)
        {
            projectileData.Initialize(direction, projectileSpeed, projectileLifetime, finalDamage, targetLayers);
            activeProjectiles[activeProjectileCount++] = projectile;
            
            // Auto-destroy after lifetime
            Destroy(projectile, projectileLifetime);
            
            // Remove from active list when destroyed
            projectile.AddComponent<ProjectileCleanup>().SetCleanupCallback(() =>
            {
                activeProjectileCount = 0;
                for (int i = 0; i < activeProjectiles.Length; i++)
                {
                    if (activeProjectiles[i] == projectile)
                    {
                        activeProjectiles[i] = null;
                        break;
                    }
                }
            });
        }
        else
        {
            Debug.LogError("[ProjectileSpawner] Projectile prefab missing ProjectileData component!");
            Destroy(projectile);
        }
    }

    /// <summary>
    /// Clear all active projectiles.
    /// </summary>
    public void ClearAllProjectiles()
    {
        for (int i = 0; i < activeProjectileCount; i++)
        {
            if (activeProjectiles[i] != null)
            {
                Destroy(activeProjectiles[i]);
            }
        }
        activeProjectileCount = 0;
    }

    /// <summary>
    /// Helper component for cleanup callback.
    /// </summary>
    private class ProjectileCleanup : MonoBehaviour
    {
        private System.Action cleanupCallback;
        
        public void SetCleanupCallback(System.Action callback)
        {
            cleanupCallback = callback;
        }
        
        private void OnDestroy()
        {
            cleanupCallback?.Invoke();
        }
    }
}

/// <summary>
/// Data and behavior for individual projectiles.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class ProjectileData : MonoBehaviour
{
    private Rigidbody rb;
    private Collider col;
    
    private Vector3 moveDirection;
    private float speed;
    private float lifetime;
    private float damage;
    private LayerMask targetLayers;
    private bool hasHit = false;

    public void Initialize(Vector3 direction, float speed, float lifetime, float damage, LayerMask targetLayers)
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        
        moveDirection = direction.normalized;
        this.speed = speed;
        this.lifetime = lifetime;
        this.damage = damage;
        this.targetLayers = targetLayers;
        
        // Set projectile as kinematic initially, then apply force
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = false;
        
        // Configure collider
        col.isTrigger = true;
    }

    private void FixedUpdate()
    {
        if (!hasHit)
        {
            rb.velocity = moveDirection * speed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        
        // Check if hit target layer
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            hasHit = true;
            rb.velocity = Vector3.zero;
            
            // Apply damage
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
            
            // Destroy projectile
            Destroy(gameObject);
        }
    }
}

/// <summary>
/// Interface for objects that can take damage.
/// </summary>
public interface IDamageable
{
    void TakeDamage(float amount);
}
