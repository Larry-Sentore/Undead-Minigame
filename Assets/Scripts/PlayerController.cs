using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Reads top-down movement input, manages player health, and collects nearby rewards. OOP principles: encapsulation and interface-based collection.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public sealed class PlayerController : MonoBehaviour, IDamageable
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxHealth = 100f;

    private Rigidbody2D body;
    private Vector2 movementInput;
    private float currentHealth;
    private float experience;

    // Announces the player's current and maximum health. Design pattern: Observer event.
    public static event Action<float, float> OnHealthChanged;

    // Gets the current player instance for enemy chasing. Design pattern: Singleton-style scene access.
    public static PlayerController Instance { get; private set; }

    // Gets the player's current health. OOP principle: encapsulation through read-only access.
    public float CurrentHealth
    {
        get { return currentHealth; }
    }

    // Gets the player's maximum health. OOP principle: encapsulation through read-only access.
    public float MaxHealth
    {
        get { return maxHealth; }
    }

    // Gets the total experience collected. OOP principle: encapsulation through read-only access.
    public float Experience
    {
        get { return experience; }
    }

    // Initializes player health and caches its physics body. OOP principle: encapsulation of setup.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A second PlayerController was found; enemy target lookup will use the first player.", this);
        }
        else
        {
            Instance = this;
        }

        body = GetComponent<Rigidbody2D>();
        maxHealth = Mathf.Max(1f, maxHealth);
        currentHealth = maxHealth;
    }

    // Clears the player scene reference when the active player is removed. OOP principle: lifecycle encapsulation.
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Reads keyboard or gamepad input for movement. OOP principle: encapsulation of input handling.
    private void Update()
    {
        movementInput = ReadMovementInput();
    }

    // Applies normalized movement using the 2D physics body. Algorithm: vector normalization prevents diagonal speed gain.
    private void FixedUpdate()
    {
        body.MovePosition(body.position + movementInput * moveSpeed * Time.fixedDeltaTime);
    }

    // Returns keyboard WASD/arrows or gamepad left-stick input. Algorithm: input-vector composition.
    private Vector2 ReadMovementInput()
    {
        Vector2 input = Vector2.zero;
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y += 1f;
        }

        Gamepad gamepad = Gamepad.current;
        if (gamepad != null && gamepad.leftStick.ReadValue().sqrMagnitude > input.sqrMagnitude)
        {
            input = gamepad.leftStick.ReadValue();
        }

        return Vector2.ClampMagnitude(input, 1f);
    }

    // Applies damage and ends the run when health reaches zero. OOP principle: interface-based polymorphism.
    public void TakeDamage(float amount)
    {
        if (currentHealth <= 0f || amount <= 0f)
        {
            return;
        }

        currentHealth = Mathf.Max(0f, currentHealth - amount);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        if (currentHealth <= 0f)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.EndGame();
            }
            else
            {
                Debug.LogError("Player reached zero health without an active GameManager.", this);
            }
        }
    }

    // Adds a non-negative experience reward. OOP principle: encapsulated state update.
    public void AddExperience(float amount)
    {
        if (amount > 0f)
        {
            experience += amount;
        }
    }

    // Collects an item that implements the shared collectible interface. OOP principle: polymorphism by interface.
    private void OnTriggerEnter2D(Collider2D other)
    {
        ICollectible collectible = other.GetComponentInParent<ICollectible>();
        if (collectible != null)
        {
            collectible.Collect(this);
        }
    }
}
