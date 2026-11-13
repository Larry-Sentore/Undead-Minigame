using System.Collections;
using UnityEngine;

/// <summary>
/// Spawns increasingly large enemy groups and tracks living enemies. OOP principle: composition with a generic registry.
/// </summary>
public sealed class WaveSpawner : MonoBehaviour
{
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private Transform player;
    [SerializeField] private int firstWaveSize = 3;
    [SerializeField] private int additionalEnemiesPerWave = 1;
    [SerializeField] private float timeBetweenWaves = 4f;
    [SerializeField] private float spawnRadius = 8f;

    private readonly ObjectRegistry<Enemy> activeEnemies = new ObjectRegistry<Enemy>();

    // Gets the registry used by auto-targeting. OOP principle: encapsulation with read-only access.
    public ObjectRegistry<Enemy> ActiveEnemies
    {
        get { return activeEnemies; }
    }

    // Begins spawning waves and listens for enemy defeats. Design pattern: Observer subscription.
    private void OnEnable()
    {
        Enemy.OnEnemyKilled += UnregisterEnemy;
    }

    // Stops listening for enemy defeats when the spawner is disabled. Design pattern: Observer cleanup.
    private void OnDisable()
    {
        Enemy.OnEnemyKilled -= UnregisterEnemy;
    }

    // Starts the wave-spawning coroutine. OOP principle: lifecycle encapsulation.
    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("WaveSpawner requires an active GameManager.", this);
            enabled = false;
            return;
        }

        if (enemyFactory == null)
        {
            Debug.LogError("WaveSpawner requires an EnemyFactory reference.", this);
            enabled = false;
            return;
        }

        if (player == null && PlayerController.Instance != null)
        {
            player = PlayerController.Instance.transform;
        }

        if (player == null)
        {
            Debug.LogError("WaveSpawner requires a player reference or an active PlayerController.", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnWaves());
    }

    // Creates waves until the run ends, increasing their size each time. Algorithm: iterative wave scheduling.
    private IEnumerator SpawnWaves()
    {
        int waveNumber = 0;
        while (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            int waveSize = Mathf.Max(1, firstWaveSize + waveNumber * additionalEnemiesPerWave);
            for (int i = 0; i < waveSize; i++)
            {
                if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
                {
                    yield break;
                }

                EnemyType type = i % 4 == 3 ? EnemyType.Skeleton : EnemyType.Zombie;
                SpawnEnemy(type);
                yield return new WaitForSeconds(0.25f);
            }

            waveNumber++;
            yield return new WaitForSeconds(Mathf.Max(0.25f, timeBetweenWaves));
        }
    }

    // Creates one enemy outside the player's immediate area and registers it. Design pattern: Factory usage.
    private void SpawnEnemy(EnemyType type)
    {
        if (player == null)
        {
            Debug.LogError("WaveSpawner lost its player reference and cannot spawn an enemy.", this);
            return;
        }

        Vector2 offset = Random.insideUnitCircle;
        if (offset.sqrMagnitude < 0.0001f)
        {
            offset = Vector2.right;
        }

        Vector2 spawnPosition = (Vector2)player.position + offset.normalized * Mathf.Max(1f, spawnRadius);
        Enemy enemy = enemyFactory.Create(type, spawnPosition);
        if (enemy != null)
        {
            activeEnemies.Register(enemy);
        }
    }

    // Removes defeated enemies from the active registry. Design pattern: Observer event handler.
    private void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Unregister(enemy);
    }
}
