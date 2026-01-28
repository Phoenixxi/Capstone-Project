using UnityEngine;

public class FallingHazard : MonoBehaviour
{
    [Header("💥 Visual Effects")]
    [Tooltip("The particle effect to spawn when the ball hits something.")]
    public GameObject shatterEffect;

    [Header("⚙️ Settings")]
    [Tooltip("Destroy the ball automatically after this time to prevent lag.")]
    public float autoDestroyTime = 5f;

    void Start()
    {
        // Failsafe: Destroy object after X seconds to prevent performance issues
        Destroy(gameObject, autoDestroyTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Check if we hit the Player
        // MAKE SURE YOUR PLAYER HAS THE TAG "Player"
        if (collision.gameObject.CompareTag("Player"))
        {
            // ================================================================
            // 👨‍💻 TODO: [PROGRAMMER] IMPLEMENT DAMAGE LOGIC HERE
            // ================================================================
            // Example: collision.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
            Debug.Log("⚡ PLAYER HIT! Apply damage logic here.");

            // Spawn visual effect and destroy
            PlayShatterEffect();
            Destroy(gameObject);
        }
        // 2. Destroy if it hits the ground (Optional, keeps scene clean)
        else
        {
            PlayShatterEffect();
            Destroy(gameObject);
        }
    }

    void PlayShatterEffect()
    {
        if (shatterEffect != null)
        {
            Instantiate(shatterEffect, transform.position, transform.rotation);
        }
    }
}