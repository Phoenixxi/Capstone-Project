using UnityEngine;
using System.Collections.Generic;

public class TrapSpawner : MonoBehaviour
{
    [Header("🔧 Settings")]
    public GameObject trapPrefab;
    public int trapCount = 5; 

    [Header("📏 Area Settings")]
    [Tooltip("Distance from edge to avoid.")]
    public float edgeMargin = 2.0f; 
    public float heightOffset = 0.02f; 

    [Header("🚫 Obstacle Avoidance (智能避障)")]
    [Tooltip("Radius to check for obstacles. Should be slightly larger than the trap.")]
    public float checkRadius = 1.5f; 

    [Tooltip("What layers count as obstacles? (e.g., Default, Props, Walls)")]
    public LayerMask obstacleLayer; 

    [Tooltip("Minimum distance between traps.")]
    public float minDistanceBetweenTraps = 2.5f; 

    [Header("👀 Debug")]
    public bool showDebugGizmos = true;

    private List<Vector3> spawnedPositions = new List<Vector3>();

    void Start()
    {
        SpawnTraps();
    }

    [ContextMenu("Spawn Now")]
    public void SpawnTraps()
    {
        if (trapPrefab == null) return;


        Collider platformCol = GetComponent<Collider>();
        if (platformCol == null) { Debug.LogError("❌ No Collider on Platform!"); return; }


        spawnedPositions.Clear();
   

        Bounds b = platformCol.bounds;
        float minX = b.min.x + edgeMargin;
        float maxX = b.max.x - edgeMargin;
        float minZ = b.min.z + edgeMargin;
        float maxZ = b.max.z - edgeMargin;
        float spawnY = b.max.y + heightOffset;

        int attempts = 0;
        int maxAttempts = trapCount * 20; 

        for (int i = 0; i < trapCount; i++)
        {
            bool validPositionFound = false;
            Vector3 candidatePos = Vector3.zero;


            while (!validPositionFound && attempts < maxAttempts)
            {
                attempts++;


                float rX = Random.Range(minX, maxX);
                float rZ = Random.Range(minZ, maxZ);
                candidatePos = new Vector3(rX, spawnY, rZ);


                if (IsPositionValid(candidatePos))
                {
                    validPositionFound = true;
                }
            }

            if (validPositionFound)
            {

                GameObject trap = Instantiate(trapPrefab, candidatePos, Quaternion.identity);
                trap.transform.SetParent(this.transform);
                spawnedPositions.Add(candidatePos);
            }
            else
            {
                Debug.LogWarning($"⚠️ Could not find empty space for trap {i}. Area might be too crowded.");
            }
        }
    }

    
    bool IsPositionValid(Vector3 pos)
    {

        foreach (var p in spawnedPositions)
        {
            if (Vector3.Distance(pos, p) < minDistanceBetweenTraps) return false;
        }


        Vector3 checkCenter = pos + Vector3.up * 1.0f; 


        if (Physics.CheckSphere(checkCenter, checkRadius, obstacleLayer))
        {
            return false; 
        }

        return true; 
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;


        Gizmos.color = Color.red;
        foreach (var pos in spawnedPositions)
        {
            Gizmos.DrawWireSphere(pos + Vector3.up * 1.0f, checkRadius);
        }


        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            Bounds b = col.bounds;
            float w = b.size.x - (edgeMargin * 2);
            float l = b.size.z - (edgeMargin * 2);
            Gizmos.color = Color.green;
            if (w > 0 && l > 0) Gizmos.DrawWireCube(b.center + Vector3.up * b.extents.y, new Vector3(w, 0.1f, l));
        }
    }
}