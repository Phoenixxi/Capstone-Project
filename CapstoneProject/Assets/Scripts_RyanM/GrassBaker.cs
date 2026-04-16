using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassBaker : MonoBehaviour
{
    public GrassData dataAsset;
    public float spacing = 0.5f;
    public float jitter = 0.2f;
    public Vector2 areaSize = new Vector2(100, 100);
    public float chunkSize = 10f; // Each 10x10m area gets its own draw call

    [ContextMenu("Bake Grass")]
    public void Bake()
    {
        dataAsset.chunks.Clear();
        // Temporary storage to group by grid coordinates
        Dictionary<Vector2Int, List<Matrix4x4>> gridGroups = new Dictionary<Vector2Int, List<Matrix4x4>>();

        Vector3 origin = transform.position;

        for (float x = -areaSize.x / 2; x < areaSize.x / 2; x += spacing)
        {
            for (float z = -areaSize.y / 2; z < areaSize.y / 2; z += spacing)
            {
                float offsetX = Random.Range(-jitter, jitter);
                float offsetZ = Random.Range(-jitter, jitter);
                Vector3 rayStart = origin + new Vector3(x + offsetX, 10f, z + offsetZ);

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, 64))
                {
                    if (hit.collider.CompareTag("Grass Area"))
                    {
                        // Calculate which "cell" this blade belongs to
                        Vector2Int gridPos = new Vector2Int(
                            Mathf.FloorToInt(hit.point.x / chunkSize),
                            Mathf.FloorToInt(hit.point.z / chunkSize)
                        );

                        if (!gridGroups.ContainsKey(gridPos))
                            gridGroups[gridPos] = new List<Matrix4x4>();

                        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);
                        gridGroups[gridPos].Add(Matrix4x4.TRS(hit.point, rot, scale));
                    }
                }
            }
        }

        // Convert groups into chunks for the Data Asset
        foreach (var group in gridGroups.Values)
        {
            // DrawMeshInstanced has a limit of 1023 per call
            // If a group is too big, split it into smaller arrays
            for (int i = 0; i < group.Count; i += 1023)
            {
                int count = Mathf.Min(1023, group.Count - i);
                Matrix4x4[] chunkArray = new Matrix4x4[count];
                group.CopyTo(i, chunkArray, 0, count);

                dataAsset.chunks.Add(new GrassChunk { matrices = chunkArray });
            }
        }

        // ... (Keep your Editor Saving logic here)
    }
}