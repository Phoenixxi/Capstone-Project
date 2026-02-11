using UnityEngine;

/// <summary>
/// Allows the boss's eye to follow the player's position
/// </summary>
public class BossEyeTracking : MonoBehaviour
{
    private Transform playerTransform;
    
    private void Awake()
    {
        playerTransform = FindFirstObjectByType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(playerTransform);
        transform.Rotate(-90f, 0f, 0f, Space.Self);
    }
}
