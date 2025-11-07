using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ShadowScale : MonoBehaviour
{
    public Transform player;
    public float heightOffset = 0.1f;

    public float baseScale = 1f;    // Shadow size on the ground
    public float minScale = 0.6f;   // Smallest shadow when high up
    public float maxHeight = 5f;    // Max height to scale shadow

    public float startingAlpha = 0.8f; // Alpha when on ground
    public float minAlpha = 0.2f;  // Alpha when at max height

    private DecalProjector decal;   // The shadow
    private Vector3 baseSize;
    private Material decalMaterial;
    private Vector3 originalScale;

    void Start()
    {
        decal = GetComponent<DecalProjector>();
        baseSize = decal.size;
        decalMaterial = decal.material;
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 rayOrigin = player.position + Vector3.up * 0.5f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 100f))
        {
            // Position of shadow blob on ground
            Vector3 shadowPosition = hit.point + Vector3.up * (0.05f *originalScale.y);
            //shadowPosition.z = player.position.z - 0.5f;
            shadowPosition.z = player.position.z + 0.1f;
            shadowPosition.x = player.position.x - 0.03f;
            transform.position = shadowPosition;

            // Get jump height
            float height = hit.distance;
            float t = Mathf.Clamp01(height / maxHeight);

            // Scale size
            float scale = Mathf.Lerp(baseScale, minScale, t);
            decal.size = new Vector3(baseSize.x * scale, baseSize.y * scale, baseSize.z);

            // Change opacity
            float alpha = Mathf.Lerp(startingAlpha, minAlpha, t);
        }
    }
}
