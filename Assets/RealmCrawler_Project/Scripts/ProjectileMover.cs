using UnityEngine;

namespace RealmCrawler.Combat
{
    [RequireComponent(typeof(Rigidbody))]
    public class ProjectileMover : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float lifetime = 5f;

        [Header("Combat")]
        [SerializeField] private float damage = 5f;
        [SerializeField] private LayerMask hitLayers;

        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            Destroy(gameObject, lifetime);
        }

        private void FixedUpdate()
        {
            rb.MovePosition(transform.position + transform.forward * speed * Time.fixedDeltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(gameObject);
            }
            else if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
            {
                Destroy(gameObject);
            }
        }

        public void SetDamage(float damageValue)
        {
            damage = damageValue;
        }

        public void SetSpeed(float speedValue)
        {
            speed = speedValue;
        }

        public float GetDamage()
        {
            return damage;
        }
    }
}
