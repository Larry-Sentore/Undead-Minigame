using UnityEngine;

/// <summary>
/// Rewards the player with experience and score once, then removes itself. OOP principle: interface implementation.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public sealed class ExpGem : MonoBehaviour, ICollectible
{
    [SerializeField] private float experienceValue = 1f;
    [SerializeField] private int scoreValue = 5;

    private bool collected;

    // Sets the experience amount granted by this gem. OOP principle: encapsulation through a controlled setter.
    public void SetExperienceValue(float value)
    {
        experienceValue = Mathf.Max(0f, value);
    }

    // Gives the player experience and score and destroys this gem. Design pattern: collectible contract abstraction.
    public void Collect(PlayerController player)
    {
        if (collected || player == null)
        {
            return;
        }

        collected = true;
        player.AddExperience(experienceValue);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddScore(scoreValue);
        }
        else
        {
            Debug.LogError("ExpGem was collected without an active GameManager.", this);
        }

        Destroy(gameObject);
    }
}
