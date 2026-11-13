using UnityEngine;

/// <summary>
/// Finds the nearest active enemy and fires automatically. Algorithm: squared Euclidean nearest-target search in O(n) time and O(1) extra space.
/// </summary>
public sealed class AutoWeapon : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float shotsPerSecond = 1f;

    private float nextShotTime;

    // Finds the nearest enemy and fires when the weapon cooldown permits. Algorithm: linear nearest-target scan.
    private void Update()
    {
        if (waveSpawner == null || projectilePrefab == null || Time.time < nextShotTime)
        {
            return;
        }

        ObjectRegistry<Enemy> registry = waveSpawner.ActiveEnemies;
        System.Collections.Generic.IReadOnlyList<Enemy> enemies = registry.ActiveObjects;
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        Enemy nearest = null;
        float nearestDistanceSquared = float.PositiveInfinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];
            if (enemy == null || enemy.IsDead)
            {
                continue;
            }

            float distanceSquared = ((Vector2)enemy.transform.position - origin).sqrMagnitude;
            if (distanceSquared < nearestDistanceSquared)
            {
                nearestDistanceSquared = distanceSquared;
                nearest = enemy;
            }
        }

        if (nearest == null)
        {
            return;
        }

        Vector2 direction = ((Vector2)nearest.transform.position - origin).normalized;
        Projectile projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.Initialize(direction, damage, transform.root);
        nextShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);
    }
}
