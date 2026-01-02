using UnityEngine;
using lilGuysNamespace;

public class HeightCheckTrapdoor : MonoBehaviour
{
    [Header("📏 Height Check Settings")]
    [Tooltip("The platform appears when Player Y > [Platform Y + this value].\nRecommended: 0.5 or 1.0 (Ensure player is fully above before closing).")]
    public float heightOffset = 0.5f;

    [Header("🚪 Door Settings")]
    [Tooltip("Should it be hidden at start? (Must be checked, otherwise the path is blocked before the player flies up).")]
    public bool startHidden = true;

    [Tooltip("Effects/Audio to play when the door closes (appears).")]
    public ParticleSystem appearEffect;
    public AudioSource audioSource;

    private Transform playerTransform;
    private Collider myCollider;
    private Renderer myRenderer;
    private bool isClosed = false;

    void Start()
    {
        // Find Player automatically
        // Note: If you use SwappingManager, you might want to update this logic if the player changes.
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        // Get own components
        myCollider = GetComponent<Collider>();
        myRenderer = GetComponent<Renderer>();

        // Initialize State
        if (startHidden)
        {
            SetDoorState(false); // Hide initially
        }
    }

    void Update()
    {
        // If door is already closed, or player not found, stop checking
        if (isClosed) return;

        // Safety check: Try to find player again if missing (in case of swapping)
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            return;
        }

        // 🔥🔥🔥 Core Logic: Height Comparison 🔥🔥🔥
        // Platform Y position
        float doorHeight = transform.position.y;

        // Player Y position
        float playerHeight = playerTransform.position.y;

        // If [Player Height] > [Door Height + Offset]
        // It means the player has flown/jumped above the platform
        if (playerHeight > (doorHeight + heightOffset))
        {
            CloseTheDoor();
        }
    }

    void CloseTheDoor()
    {
        isClosed = true;
        SetDoorState(true); // Show door, enable collision

        // Play VFX/SFX
        if (appearEffect != null) appearEffect.Play();
        if (audioSource != null) audioSource.Play();

        // Debug.Log("🚪 Player passed height check, closing trapdoor!");
    }

    // Unified control for Show/Hide
    void SetDoorState(bool active)
    {
        // Control Collider (Prevent hitting head when invisible)
        if (myCollider != null) myCollider.enabled = active;

        // Control Visuals (Prevent seeing it before going up)
        if (myRenderer != null) myRenderer.enabled = active;

        // Control children objects if any
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    void OnDrawGizmos()
    {
        // Draw a line to visualize the trigger threshold
        Gizmos.color = Color.yellow;
        Vector3 linePos = transform.position;
        linePos.y += heightOffset;

        Gizmos.DrawLine(linePos + Vector3.left * 2, linePos + Vector3.right * 2);
        // Gizmos.DrawIcon(linePos, "DoorThreshold"); // Optional icon
    }
}