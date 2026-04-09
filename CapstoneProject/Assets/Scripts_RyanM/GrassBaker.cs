using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GrassBaker : MonoBehaviour
{
    public GrassData dataAsset;
    public float spacing = 0.5f; // Distance between rays
    public float jitter = 0.2f;  // Random offset so it's not a perfect grid
    public Vector2 areaSize = new Vector2(100, 100);

    [ContextMenu("Bake Grass")]
    public void Bake()
    {
        dataAsset.matrices.Clear();
        Vector3 origin = transform.position;

        for (float x = -areaSize.x / 2; x < areaSize.x / 2; x += spacing)
        {
            for (float z = -areaSize.y / 2; z < areaSize.y / 2; z += spacing)
            {

                // Add a little randomness to the position
                float offsetX = Random.Range(-jitter, jitter);
                float offsetZ = Random.Range(-jitter, jitter);
                Vector3 rayStart = origin + new Vector3(x + offsetX, 10f, z + offsetZ);

                if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 20f, 64))
                {
                    if (hit.collider.CompareTag("Grass Area"))
                    {
                        // Create a matrix with random Y rotation and slight scale variation
                        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        Vector3 scale = Vector3.one * Random.Range(0.8f, 1.2f);

                        dataAsset.matrices.Add(Matrix4x4.TRS(hit.point, rot, scale));

                    }
                }
            }
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(dataAsset);
        AssetDatabase.SaveAssets();
#endif
        Debug.Log($"Baking complete! Saved {dataAsset.matrices.Count} positions.");
    }
}