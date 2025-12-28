using UnityEngine;
using System.Collections.Generic;

public class TrapSpawner : MonoBehaviour
{
    [Header("🔧 Settings")]
    [Tooltip("Drag your Hole_Trap prefab here.")]
    public GameObject trapPrefab; // 拖入你的陷阱预制体

    [Tooltip("How many traps to spawn?")]
    public int trapCount = 3; // 生成几个？

    [Tooltip("Distance from the edge where traps WON'T spawn.")]
    public float edgeMargin = 3.0f; // ✨ 边缘保留距离 (比如设为3，边缘3米内不生成)

    [Tooltip("Minimum distance between traps (to prevent overlapping).")]
    public float minDistanceBetweenTraps = 2.0f; // 陷阱之间的最小距离，防止重叠

    [Tooltip("Height offset (lift up slightly to avoid z-fighting).")]
    public float heightOffset = 0.02f;

    [Header("👀 Debug")]
    public bool showSpawnArea = true; // 在Scene窗口显示生成范围框

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnTraps();
    }

    [ContextMenu("Spawn Now")] // 你可以在运行游戏时右键组件点这个来测试
    public void SpawnTraps()
    {
        if (trapPrefab == null)
        {
            Debug.LogError("❌ TrapSpawner: No Prefab assigned!");
            return;
        }

        // Get the platform's collider (assuming it's a BoxCollider or similar)
        Collider platformCollider = GetComponent<Collider>();
        if (platformCollider == null)
        {
            Debug.LogError("❌ TrapSpawner: Platform has no Collider!");
            return;
        }

        // Clear old list
        spawnedPositions.Clear();

        // Calculate valid bounds
        Bounds bounds = platformCollider.bounds;

        // Define the spawnable area (Platform Size - Margin)
        // We use min/max world coordinates
        float minX = bounds.min.x + edgeMargin;
        float maxX = bounds.max.x - edgeMargin;
        float minZ = bounds.min.z + edgeMargin;
        float maxZ = bounds.max.z - edgeMargin;

        // Y position is the top of the collider
        float spawnY = bounds.max.y + heightOffset;

        // Check if margin is too big
        if (minX >= maxX || minZ >= maxZ)
        {
            Debug.LogError("❌ TrapSpawner: Edge Margin is too big! No space left in the middle.");
            return;
        }

        int attempts = 0;
        int maxAttempts = trapCount * 10; // Prevent infinite loop

        for (int i = 0; i < trapCount; i++)
        {
            bool positionFound = false;
            Vector3 candidatePos = Vector3.zero;

            // Try to find a position that isn't too close to others
            while (!positionFound && attempts < maxAttempts)
            {
                attempts++;
                float rX = Random.Range(minX, maxX);
                float rZ = Random.Range(minZ, maxZ);
                candidatePos = new Vector3(rX, spawnY, rZ);

                if (IsPositionValid(candidatePos))
                {
                    positionFound = true;
                }
            }

            if (positionFound)
            {
                // Instantiate the trap
                GameObject newTrap = Instantiate(trapPrefab, candidatePos, Quaternion.identity);

                // Optional: Make it a child of the platform so it moves with it
                newTrap.transform.SetParent(this.transform);

                // Add to list
                spawnedPositions.Add(candidatePos);
            }
            else
            {
                Debug.LogWarning("⚠️ Could not find a spot for trap " + i);
            }
        }
    }

    bool IsPositionValid(Vector3 pos)
    {
        foreach (Vector3 existingPos in spawnedPositions)
        {
            if (Vector3.Distance(pos, existingPos) < minDistanceBetweenTraps)
            {
                return false; // Too close to another trap
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        if (!showSpawnArea) return;

        Collider col = GetComponent<Collider>();
        if (col == null) return;

        Bounds b = col.bounds;

        // Calculate the inner rectangle
        float w = b.size.x - (edgeMargin * 2);
        float l = b.size.z - (edgeMargin * 2);

        if (w <= 0 || l <= 0) return;

        Gizmos.color = Color.green;
        // Center of the valid area
        Vector3 center = b.center;
        center.y = b.max.y; // Draw on top

        // Draw the spawn zone
        Gizmos.DrawWireCube(center, new Vector3(w, 0.1f, l));
    }
}