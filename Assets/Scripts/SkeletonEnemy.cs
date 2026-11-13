using UnityEngine;

/// <summary>
/// Moves in short fast bursts and resists part of incoming damage. OOP principles: inheritance and polymorphism.
/// </summary>
public sealed class SkeletonEnemy : Enemy
{
    [SerializeField] private float skeletonMoveSpeed = 1.2f;
    [SerializeField] private float burstSpeed = 4f;
    [SerializeField] private float burstDuration = 0.3f;
    [SerializeField] private float burstInterval = 1.5f;
    [SerializeField] protected float armorReduction = 0.2f;

    private float burstTimeRemaining;
    private float burstCooldownRemaining;
    private SpriteRenderer spriteRenderer;

    // Initializes shared enemy state and caches the sprite renderer. OOP principle: inherited setup.
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = skeletonMoveSpeed;
        burstDuration = Mathf.Max(0.05f, burstDuration);
        burstInterval = Mathf.Max(burstDuration, burstInterval);
        armorReduction = Mathf.Clamp01(armorReduction);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Applies armor reduction before using shared health handling. OOP principle: method overriding and polymorphism.
    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount * (1f - armorReduction));
    }

    // Chases the target and periodically accelerates for a short burst. Algorithm: timed state-based movement.
    public override void Move(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - Body.position).normalized;
        float speed = moveSpeed;
        if (burstTimeRemaining > 0f)
        {
            speed = burstSpeed;
            burstTimeRemaining -= Time.fixedDeltaTime;
        }
        else
        {
            burstCooldownRemaining -= Time.fixedDeltaTime;
            if (burstCooldownRemaining <= 0f)
            {
                burstTimeRemaining = burstDuration;
                burstCooldownRemaining = burstInterval;
                speed = burstSpeed;
            }
        }

        Body.MovePosition(Body.position + direction * speed * Time.fixedDeltaTime);
        if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.001f)
        {
            spriteRenderer.flipX = direction.x < 0f;
        }
    }
}
