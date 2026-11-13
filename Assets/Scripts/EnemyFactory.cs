using UnityEngine;

/// <summary>
/// Identifies the concrete enemy prefab requested from the factory. Design pattern: Factory selection.
/// </summary>
public enum EnemyType
{
    Zombie,
    Skeleton
}

/// <summary>
/// Creates enemy instances without making the spawner depend on concrete enemy classes. Design pattern: Factory.
/// </summary>
public sealed class EnemyFactory : MonoBehaviour
{
    [SerializeField] private ZombieEnemy zombiePrefab;
    [SerializeField] private SkeletonEnemy skeletonPrefab;

    // Creates the selected enemy at a world position. Design pattern: Factory method.
    public Enemy Create(EnemyType enemyType, Vector2 position)
    {
        Enemy prefab;
        switch (enemyType)
        {
            case EnemyType.Zombie:
                prefab = zombiePrefab;
                break;
            case EnemyType.Skeleton:
                prefab = skeletonPrefab;
                break;
            default:
                Debug.LogError("EnemyFactory received an unsupported enemy type.", this);
                return null;
        }

        if (prefab == null)
        {
            Debug.LogError("EnemyFactory is missing the prefab for " + enemyType + ".", this);
            return null;
        }

        return Instantiate(prefab, position, Quaternion.identity);
    }
}
