using UnityEngine;

/// <summary>
/// Travels in a straight line and damages the first compatible object it touches. OOP principle: interface-based polymorphism.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public sealed class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 8f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifetime = 4f;

    private Vector2 direction = Vector2.right;
    private Transform owner;

    // Sets this projectile's travel direction, damage, and owner. OOP principle: controlled object initialization.
    public void Initialize(Vector2 travelDirection, float damageAmount, Transform projectileOwner)
    {
        direction = travelDirection.sqrMagnitude > 0f ? travelDirection.normalized : Vector2.right;
        damage = Mathf.Max(0f, damageAmount);
        owner = projectileOwner;
    }

    // Advances the projectile and removes it when its lifetime expires. Algorithm: constant-velocity movement.
    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    // Damages a touched IDamageable object and removes the projectile. OOP principle: interface-based polymorphism.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (owner != null && (other.transform == owner || other.transform.IsChildOf(owner)))
        {
            return;
        }

        IDamageable target = other.GetComponentInParent<IDamageable>();
        if (target == null)
        {
            return;
        }

        target.TakeDamage(damage);
        Destroy(gameObject);
    }
}
