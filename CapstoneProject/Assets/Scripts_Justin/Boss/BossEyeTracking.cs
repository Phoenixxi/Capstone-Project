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

    void Update()
    {
        transform.LookAt(playerTransform);
        Vector3 angleRotations = transform.rotation.eulerAngles;
        angleRotations.x = -95;
        transform.rotation = Quaternion.Euler(angleRotations);
        //transform.Rotate(-90f, 0f, 0f, Space.Self);
        //if (angleRotations.x > 270) transform.rotation = Quaternion.Euler(270, angleRotations.y, angleRotations.z);
        Debug.Log(transform.rotation.eulerAngles.x);
    }
}