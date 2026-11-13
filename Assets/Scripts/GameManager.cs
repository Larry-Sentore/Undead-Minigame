using System;
using UnityEngine;

/// <summary>
/// Owns the current run's score, kills, timer, and game-over state. Design patterns: Singleton and Observer publisher.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    [SerializeField] private LeaderboardManager leaderboardManager;

    // Announces changes to run statistics. Design pattern: Observer event.
    public static event Action OnStatsChanged;

    // Announces that the run has ended. Design pattern: Observer event.
    public static event Action OnGameOver;

    // Gets the active run manager. Design pattern: Singleton access point.
    public static GameManager Instance { get; private set; }

    // Gets the current score. OOP principle: encapsulation through read-only access.
    public int Score { get; private set; }

    // Gets the number of enemies defeated. OOP principle: encapsulation through read-only access.
    public int KillCount { get; private set; }

    // Gets the number of seconds survived. OOP principle: encapsulation through read-only access.
    public float SurvivalTime { get; private set; }

    // Gets whether the run has ended. OOP principle: encapsulation through read-only access.
    public bool IsGameOver { get; private set; }

    // Establishes the single active manager and resets global time for a new run. Design pattern: Singleton.
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A second GameManager was found and will be removed.", this);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        Time.timeScale = 1f;
        leaderboardManager = leaderboardManager != null
            ? leaderboardManager
            : FindFirstObjectByType<LeaderboardManager>();
    }

    // Subscribes to defeated enemies while this manager is active. Design pattern: Observer subscription.
    private void OnEnable()
    {
        if (Instance == this)
        {
            Enemy.OnEnemyKilled += HandleEnemyKilled;
        }
    }

    // Removes event subscriptions and clears this singleton when it is disabled. Design pattern: Observer cleanup.
    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= HandleEnemyKilled;
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Advances the survival timer during an active run. Algorithm: frame-based time accumulation.
    private void Update()
    {
        if (!IsGameOver)
        {
            SurvivalTime += Time.deltaTime;
        }
    }

    // Adds a non-negative number of points to the current run. OOP principle: encapsulated state update.
    public void AddScore(int amount)
    {
        if (IsGameOver || amount <= 0)
        {
            return;
        }

        Score += amount;
        OnStatsChanged?.Invoke();
    }

    // Ends the run and persists its score once. Design pattern: Singleton coordinates the game-over transition.
    public void EndGame()
    {
        if (IsGameOver)
        {
            return;
        }

        IsGameOver = true;
        if (leaderboardManager == null)
        {
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
        }

        if (leaderboardManager != null)
        {
            leaderboardManager.SubmitScore(Score);
        }
        else
        {
            Debug.LogError("GameManager cannot save the run because no LeaderboardManager exists in the scene.", this);
        }

        OnStatsChanged?.Invoke();
        OnGameOver?.Invoke();
        Time.timeScale = 0f;
    }

    // Adds the defeated enemy's points and increments the kill count. Design pattern: Observer event handler.
    private void HandleEnemyKilled(Enemy enemy)
    {
        if (IsGameOver || enemy == null)
        {
            return;
        }

        KillCount++;
        Score += Mathf.Max(0, enemy.ScoreValue);
        OnStatsChanged?.Invoke();
    }
}
