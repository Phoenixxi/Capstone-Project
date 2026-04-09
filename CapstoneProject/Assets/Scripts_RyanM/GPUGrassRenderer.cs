using System.Collections.Generic;
using UnityEngine;

public class GPUGrassRenderer : MonoBehaviour
{
    public Mesh grassMesh;
    public Material grassMaterial;
    public GrassData dataAsset; // Your scriptable object with the list
    public float cullDistance = 50f; // Adjust this in the Inspector

    private List<Matrix4x4[]> matrixChunks = new List<Matrix4x4[]>();

    void Start()
    {
        PrepareChunks();
    }

    void PrepareChunks()
    {
        matrixChunks.Clear();
        int total = dataAsset.matrices.Count;

        // Loop through all matrices and group them into sets of 1023
        for (int i = 0; i < total; i += 1023)
        {
            int chunkSize = Mathf.Min(1023, total - i);
            Matrix4x4[] chunk = new Matrix4x4[chunkSize];

            for (int j = 0; j < chunkSize; j++)
            {
                chunk[j] = dataAsset.matrices[i + j];
            }
            matrixChunks.Add(chunk);
        }
    }

    void Update()
    {
        if (grassMesh == null || grassMaterial == null || matrixChunks.Count == 0) return;

        Vector3 camPos = Camera.main.transform.position;
        // We use Square Magnitude because it's faster for the CPU than calculating Square Root
        float sqrCullDistance = cullDistance * cullDistance;

        foreach (var chunk in matrixChunks)
        {
            // Extract the position of the first blade in the chunk to represent the whole group
            // In a 4x4 matrix, the 4th column (m03, m13, m23) holds the world position
            Vector3 chunkPos = new Vector3(chunk[0].m03, chunk[0].m13, chunk[0].m23);

            float sqrDist = (camPos - chunkPos).sqrMagnitude;

            if (sqrDist < sqrCullDistance)
            {
                Graphics.DrawMeshInstanced(grassMesh, 0, grassMaterial, chunk, chunk.Length, null, UnityEngine.Rendering.ShadowCastingMode.Off, false);
            }
        }
    }
}