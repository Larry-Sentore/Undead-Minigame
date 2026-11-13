using System;
using UnityEngine;

/// <summary>
/// Shares health, damage, rewards, and movement setup for all enemy kinds. OOP principles: encapsulation, inheritance, and polymorphism; pattern: Observer subject.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public abstract class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private float contactDamage = 10f;
    [SerializeField] private float contactDamageInterval = 1f;
    [SerializeField] private ExpGem expGemPrefab;
    [SerializeField] private float experienceReward = 1f;

    protected float currentHealth;
    [SerializeField] protected float moveSpeed = 1.5f;
    [SerializeField] protected int scoreValue = 10;

    private Rigidbody2D body;
    private float nextContactDamageTime;

    // Announces that an enemy has been defeated. Design pattern: Observer event.
    public static event Action<Enemy> OnEnemyKilled;

    // Gets the enemy's current health. OOP principle: encapsulation through read-only access.
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    // Gets the points awarded for defeating this enemy. OOP principle: encapsulation through read-only access.
    public int ScoreValue
    {
        get { return scoreValue; }
    }

    // Gets whether this enemy has been defeated. OOP principle: encapsulation through read-only access.
    public bool IsDead
    {
        get { return currentHealth <= 0f; }
    }

    // Initializes health and caches the required physics component. OOP principle: encapsulation of setup.
    protected virtual void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
        contactDamageInterval = Mathf.Max(0.1f, contactDamageInterval);
    }

    // Moves this enemy toward its target using its concrete movement behavior. OOP principle: polymorphism.
    protected virtual void FixedUpdate()
    {
        if (!IsDead && PlayerController.Instance != null)
        {
            Move(PlayerController.Instance.transform);
        }
    }

    // Gets the cached physics body for derived enemy movement. OOP principle: protected inheritance interface.
    protected Rigidbody2D Body
    {
        get { return body; }
    }

    // Reduces health and defeats the enemy when health reaches zero. OOP principle: polymorphic virtual behavior.
    public virtual void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        if (IsDead)
        {
            Die();
        }
    }

    // Defines movement for each concrete enemy type. OOP principle: abstraction and polymorphism.
    public abstract void Move(Transform target);

    // Spawns the reward and notifies listeners once when this enemy is defeated. Design pattern: Observer.
    protected virtual void Die()
    {
        if (IsDead && expGemPrefab != null)
        {
            ExpGem gem = Instantiate(expGemPrefab, transform.position, Quaternion.identity);
            gem.SetExperienceValue(experienceReward);
        }

        OnEnemyKilled?.Invoke(this);
        Destroy(gameObject);
    }

    // Damages the player at a limited rate while the enemy touches them. OOP principle: interaction through component contracts.
    protected virtual void OnCollisionStay2D(Collision2D collision)
    {
        PlayerController player = collision.collider.GetComponentInParent<PlayerController>();
        if (player == null || Time.time < nextContactDamageTime || IsDead)
        {
            return;
        }

        player.TakeDamage(contactDamage);
        nextContactDamageTime = Time.time + contactDamageInterval;
    }
}
