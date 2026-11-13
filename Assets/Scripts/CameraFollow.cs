using UnityEngine;

/// <summary>
/// Keeps the 2D camera centered near the player. OOP principle: encapsulation of camera behavior.
/// </summary>
public sealed class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.12f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Vector3 currentVelocity;

    // Finds the player when no target has been assigned in the Inspector. OOP principle: lifecycle encapsulation.
    private void Start()
    {
        if (target == null && PlayerController.Instance != null)
        {
            target = PlayerController.Instance.transform;
        }

        if (target == null)
        {
            Debug.LogError("CameraFollow requires a target or an active PlayerController.", this);
        }
    }

    // Smoothly follows the target after physics and gameplay updates. Algorithm: damped camera interpolation.
    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 destination = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            destination,
            ref currentVelocity,
            Mathf.Max(0.01f, smoothTime));
    }
}
