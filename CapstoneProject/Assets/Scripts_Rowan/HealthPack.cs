using UnityEngine;
using lilGuysNamespace;

public class HealthPack : MonoBehaviour
{
    public int healAmount = 25; // how much health to restore

    private void OnTriggerEnter(Collider other)  // Use Collider2D if in 2D
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();
        if (player != null)
        {
            player.Heal(healAmount);
            Destroy(gameObject); // remove the health pack after use
        }
    }
}
