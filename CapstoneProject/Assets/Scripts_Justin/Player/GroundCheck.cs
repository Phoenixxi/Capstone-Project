using UnityEngine;

/// <summary>
/// Tracks whether or not the player is grounded
/// </summary>
public class GroundCheck : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        player.SetIsGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        player.SetIsGrounded(false);
    }
}
