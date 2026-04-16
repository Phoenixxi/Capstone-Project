using System.Collections.Generic;
using UnityEngine;

public class GPUGrassRenderer : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public GrassData dataAsset;
    public float cullDistance = 50f;

    void Update()
    {
        // Safety checks
        if (grassMesh == null || grassMaterial == null || dataAsset == null || dataAsset.chunks.Count == 0)
            return;

        Vector3 camPos = Camera.main.transform.position;
        float sqrCullDistance = cullDistance * cullDistance;

        foreach (var chunk in dataAsset.chunks)
        {
            // Ensure the chunk actually has matrices to avoid index errors
            if (chunk.matrices == null || chunk.matrices.Length == 0) continue;

            // Get the position of the first blade in this specific chunk
            // Using GetColumn(3) is a cleaner way to get the position from a Matrix4x4
            Vector3 chunkPos = chunk.matrices[0].GetColumn(3);

            float sqrDist = (camPos - chunkPos).sqrMagnitude;

            if (sqrDist < sqrCullDistance)
            {
                // Draw this specific chunk
                Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, chunk.matrices, chunk.matrices.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
            }
        }
    }
}
