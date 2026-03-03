using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // Automatically adds a Rigidbody if missing
public class FallingHazard : MonoBehaviour
{
    [Header("Physics Acceleration")]
    [Tooltip("How much extra gravity to apply. 1 = Normal Earth gravity. 5 = Heavy Rock.")]
    public float gravityScale = 4.0f; // Default 4x gravity for fast acceleration

    [Tooltip("Force initial downward velocity? Useful if you want it to launch down immediately.")]
    public float initialDownwardSpeed = 0f;

    [Header("Visual Effects")]
    [Tooltip("The particle effect to spawn when the ball hits something.")]
    public GameObject shatterEffect;

    [Header("Settings")]
    [Tooltip("Destroy the ball automatically after this time.")]
    public float autoDestroyTime = 5f;
    public float damage;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            // IMPORTANT: Remove air resistance so it keeps accelerating!
            // Note: In Unity 6, 'drag' is renamed to 'linearDamping'.
            rb.linearDamping = 0f;
            rb.linearDamping = 0f; // For older Unity versions, keeps it compatible

            // Optional: Give it an initial push downwards
            if (initialDownwardSpeed > 0)
            {
                rb.linearVelocity = Vector3.down * initialDownwardSpeed;
                // rb.velocity = Vector3.down * initialDownwardSpeed; // For older Unity
            }
        }

        Destroy(gameObject, autoDestroyTime);
    }

    // FixedUpdate is used for Physics calculations
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Apply custom gravity acceleration
            // Formula: F = m * a (Unity handles mass automatically in ForceMode.Acceleration)
            // We subtract 1 from gravityScale because Unity already applies 1x gravity by default
            Vector3 extraGravityForce = Physics.gravity * (gravityScale - 1.0f);
            rb.AddForce(extraGravityForce, ForceMode.Acceleration);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. Check tags
        if (collision.gameObject.CompareTag("Player"))
        {
            // ================================================================
            // TODO: [PROGRAMMER] DAMAGE LOGIC HERE
            // ================================================================
            collision.gameObject.GetComponentInChildren<EntityManager>().TakeDamage(damage, lilGuysNamespace.EntityData.ElementType.Normal);
            Debug.Log("HIT PLAYER");

            PlayShatterEffect();
            Destroy(gameObject);
        }
        else
        {
            // Hit ground/wall
            Debug.Log("HIT GROUND");
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