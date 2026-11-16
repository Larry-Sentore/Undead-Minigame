using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Fires automatically at the nearest enemy and continuously toward the mouse. Algorithm: squared-distance nearest-target search in O(n) time and space.
/// </summary>
public sealed class AutoWeapon : MonoBehaviour
{
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform aimPivot;
    [SerializeField] private Camera aimCamera;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float shotsPerSecond = 1f;
    [SerializeField] private float mouseShotsPerSecond = 3f;
    [SerializeField] private float aimAngleOffset;

    private float nextTargetShotTime;
    private float nextMouseShotTime;

    private void Awake()
    {
        if (aimCamera == null)
        {
            aimCamera = Camera.main;
        }

        if (projectilePrefab == null)
        {
            Debug.LogError("AutoWeapon requires a projectile prefab.", this);
        }

        if (aimCamera == null)
        {
            Debug.LogWarning("AutoWeapon has no camera; continuous shots will use the weapon's facing direction.", this);
        }
    }

    private void Update()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        FireContinuousBullets();
        FireAtNearestEnemy();
    }

    private void FireContinuousBullets()
    {
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        bool hasMouseAim = TryGetMouseWorldPosition(out Vector2 mouseWorldPosition);
        if (hasMouseAim && aimPivot != null)
        {
            Vector2 pivotDirection = mouseWorldPosition - (Vector2)aimPivot.position;
            if (pivotDirection.sqrMagnitude > 0.0001f)
            {
                float pivotAngle = Mathf.Atan2(pivotDirection.y, pivotDirection.x) * Mathf.Rad2Deg + aimAngleOffset;
                aimPivot.rotation = Quaternion.Euler(0f, 0f, pivotAngle);
            }
        }

        Vector2 direction = hasMouseAim
            ? mouseWorldPosition - origin
            : aimPivot != null ? (Vector2)aimPivot.right : (Vector2)transform.right;
        if (direction.sqrMagnitude <= 0.0001f)
        {
            direction = Vector2.right;
        }

        if (Time.unscaledTime < nextMouseShotTime)
        {
            return;
        }

        Shoot(origin, direction, true);
        nextMouseShotTime = Time.unscaledTime + 1f / Mathf.Max(0.1f, mouseShotsPerSecond);
    }

    private void FireAtNearestEnemy()
    {
        if (Time.time < nextTargetShotTime)
        {
            return;
        }

        nextTargetShotTime = Time.time + 1f / Mathf.Max(0.1f, shotsPerSecond);
        Vector2 origin = firePoint != null ? (Vector2)firePoint.position : (Vector2)transform.position;
        Enemy[] enemies = FindObjectsByType<Enemy>();
        Enemy nearest = null;
        float nearestDistanceSquared = float.PositiveInfinity;

        for (int i = 0; i < enemies.Length; i++)
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

        Vector2 direction = (Vector2)nearest.transform.position - origin;
        Shoot(origin, direction);
    }

    private bool TryGetMouseWorldPosition(out Vector2 mouseWorldPosition)
    {
        mouseWorldPosition = Vector2.zero;
        if (aimCamera == null || Mouse.current == null)
        {
            return false;
        }

        Ray mouseRay = aimCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane aimPlane = new Plane(Vector3.forward, new Vector3(0f, 0f, transform.position.z));
        if (!aimPlane.Raycast(mouseRay, out float distance))
        {
            return false;
        }

        mouseWorldPosition = mouseRay.GetPoint(distance);
        return true;
    }

    private void Shoot(Vector2 origin, Vector2 direction, bool useUnscaledTime = false)
    {
        direction.Normalize();
        float projectileAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Projectile projectile = Instantiate(
            projectilePrefab,
            origin,
            Quaternion.Euler(0f, 0f, projectileAngle));
        projectile.Initialize(direction, damage, transform.root, useUnscaledTime);
    }
}
