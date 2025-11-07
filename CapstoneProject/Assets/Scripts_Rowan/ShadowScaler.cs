using UnityEngine;

public class ShadowScaler : MonoBehaviour
{
    public Transform player; 

    public float baseScale = 1f;    // Size of shadow when player is on ground
    public float minScale = 0.3f;   // Smallest size when player is high up
    public float maxHeight = 5f;    // Max height at which shadow is smallest
    public float baseAlpha = 1.5f;  // Opacity when player is on ground
    public float minAlpha = 0.2f;   // Opacity when at max height

    private Vector3 originalScale;
    private Material shadowMaterial;

    void Start()
    {
        originalScale = transform.localScale;
        shadowMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        Vector3 rayOrigin = player.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            // Keep shadow on ground
            Vector3 shadowPosition = hit.point + Vector3.up * (0.05f *originalScale.y); // Fixes the ground clipping
            shadowPosition.z = player.position.z;

            transform.position = shadowPosition;

            // Adjust scale based on height
            float height = hit.distance;
            float t = Mathf.Clamp01(height / maxHeight);

            float scale = Mathf.Lerp(baseScale, minScale, t);
            transform.localScale = originalScale * scale;

            // Change alpha based on distance from shadow
            float alpha = Mathf.Lerp(baseAlpha, minAlpha, t);
            Color color = shadowMaterial.color;
            color.a = alpha;
            shadowMaterial.color = color;
            
        }
    }
}
