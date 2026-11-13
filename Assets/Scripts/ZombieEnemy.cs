using UnityEngine;

/// <summary>
/// Chases the player at a steady speed and faces the direction it travels. OOP principles: inheritance and polymorphism.
/// </summary>
public sealed class ZombieEnemy : Enemy
{
    [SerializeField] private float zombieMoveSpeed = 1.5f;
    private SpriteRenderer spriteRenderer;

    // Initializes the shared enemy state and caches the sprite renderer. OOP principle: inherited setup.
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = zombieMoveSpeed;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Moves steadily toward the player and flips the sprite when moving left. Algorithm: normalized vector movement.
    public override void Move(Transform target)
    {
        if (target == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)target.position - Body.position).normalized;
        Body.MovePosition(Body.position + direction * moveSpeed * Time.fixedDeltaTime);
        if (spriteRenderer != null && Mathf.Abs(direction.x) > 0.001f)
        {
            spriteRenderer.flipX = direction.x < 0f;
        }
    }
}
