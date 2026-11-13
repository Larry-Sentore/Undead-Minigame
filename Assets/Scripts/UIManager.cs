using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Updates the HUD and game-over leaderboard in response to gameplay events. Design pattern: Observer.
/// </summary>
public sealed class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text killCountText;
    [SerializeField] private TMP_Text survivalTimeText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text leaderboardText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private PlayerController player;
    [SerializeField] private LeaderboardManager leaderboardManager;

    // Subscribes to gameplay events and renders the initial HUD. Design pattern: Observer subscription.
    private void OnEnable()
    {
        Enemy.OnEnemyKilled += HandleEnemyKilled;
        PlayerController.OnHealthChanged += HandleHealthChanged;
        GameManager.OnStatsChanged += RefreshStats;
        GameManager.OnGameOver += HandleGameOver;

        if (player == null)
        {
            player = FindFirstObjectByType<PlayerController>();
        }

        if (leaderboardManager == null)
        {
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
        }

        RefreshStats();
        if (player != null)
        {
            HandleHealthChanged(player.CurrentHealth, player.MaxHealth);
        }

        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            HandleGameOver();
        }
        else if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // Removes event subscriptions when the HUD is disabled. Design pattern: Observer cleanup.
    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= HandleEnemyKilled;
        PlayerController.OnHealthChanged -= HandleHealthChanged;
        GameManager.OnStatsChanged -= RefreshStats;
        GameManager.OnGameOver -= HandleGameOver;
    }

    // Refreshes the survival-time display while a run is active. OOP principle: encapsulation of presentation logic.
    private void Update()
    {
        RefreshStats();
    }

    // Updates the score, kill count, and survival-time labels. Design pattern: Observer response.
    private void RefreshStats()
    {
        GameManager manager = GameManager.Instance;
        if (manager == null)
        {
            return;
        }

        if (scoreText != null) scoreText.text = "Score: " + manager.Score;
        if (killCountText != null) killCountText.text = "Kills: " + manager.KillCount;
        if (survivalTimeText != null)
        {
            int totalSeconds = Mathf.FloorToInt(manager.SurvivalTime);
            survivalTimeText.text = string.Format("Time: {0:00}:{1:00}", totalSeconds / 60, totalSeconds % 60);
        }
    }

    // Refreshes run labels after an enemy defeat. Design pattern: Observer event handler.
    private void HandleEnemyKilled(Enemy enemy)
    {
        RefreshStats();
    }

    // Shows the player's latest health. Design pattern: Observer event handler.
    private void HandleHealthChanged(float currentHealth, float maximumHealth)
    {
        if (healthText != null)
        {
            healthText.text = "HP: " + Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maximumHealth);
        }
    }

    // Opens the game-over panel and writes the five best saved scores. Algorithm: displays the sorted Merge Sort result.
    private void HandleGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (leaderboardText == null)
        {
            return;
        }

        if (leaderboardManager == null)
        {
            leaderboardManager = FindFirstObjectByType<LeaderboardManager>();
        }

        if (leaderboardManager == null)
        {
            Debug.LogError("UIManager cannot display the leaderboard because no LeaderboardManager exists.", this);
            return;
        }

        List<int> topScores = leaderboardManager.GetTopScores(5);
        StringBuilder text = new StringBuilder("Top Scores\n");
        for (int i = 0; i < topScores.Count; i++)
        {
            text.Append(i + 1).Append(". ").Append(topScores[i]).Append('\n');
        }

        leaderboardText.text = text.ToString();
    }
}
