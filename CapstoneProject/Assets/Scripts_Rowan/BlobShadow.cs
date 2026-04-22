using UnityEngine;
 
public class BlobShadow : MonoBehaviour
{
    [Header("References")]
    public Transform player;
 
    [Header("Shadow Settings")]
    public float baseScale = 1.0f;
    public float minScale = 0.3f;
    public float maxHeight = 10f;
    public float groundOffset = 0.02f;
 
    [Header("Opacity Settings")]
    public float maxAlpha = 0.8f;
    public float minAlpha = 0.1f;
 
    private MeshRenderer meshRenderer;
    private Material shadowMaterial;
    private int raycastMask;
 
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            shadowMaterial = meshRenderer.material;
 
        // If no player assigned, try to find parent
        if (player == null && transform.parent != null)
            player = transform.parent;
 
        // Only raycast against Default and Ground layers
        raycastMask = LayerMask.GetMask("Default", "Ground");
    }
 
    void LateUpdate()
    {
        if (player == null) return;
 
        // Raycast straight down from player to find any surface below
        RaycastHit hit;
        if (Physics.Raycast(player.position, Vector3.down, out hit, maxHeight, raycastMask))
        {
            // Position shadow just above the surface it hit
            transform.position = hit.point + hit.normal * groundOffset;
 
            // Align shadow to the surface normal, accounting for Quad's 90 degree X rotation
            transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * Quaternion.Euler(90f, 0f, 0f);
 
            // Calculate how far player is from the ground (0 = on ground, 1 = at maxHeight)
            float heightRatio = Mathf.Clamp01(hit.distance / maxHeight);
 
            // Scale down as player gets higher
            float currentScale = Mathf.Lerp(baseScale, minScale, heightRatio);
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);
 
            // Fade out as player gets higher
            if (shadowMaterial != null)
            {
                float currentAlpha = Mathf.Lerp(maxAlpha, minAlpha, heightRatio);
                Color col = shadowMaterial.color;
                col.a = currentAlpha;
                shadowMaterial.color = col;
            }
 
            // Show shadow
            meshRenderer.enabled = true;
        }
        else
        {
            // No surface found, hide shadow
            //meshRenderer.enabled = false;
        }
    }
}