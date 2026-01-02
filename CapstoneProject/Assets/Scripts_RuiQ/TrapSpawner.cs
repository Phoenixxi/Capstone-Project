using UnityEngine;
using System.Collections.Generic;

public class TrapSpawner : MonoBehaviour
{
    [Header("🔧 Settings")]
    public GameObject trapPrefab; // 拖入 Hole_Trap 预制体
    public int trapCount = 5;     // 生成数量

    [Header("📏 Area Settings")]
    [Tooltip("Distance from edge to avoid.")]
    public float edgeMargin = 2.0f; // 边缘安全距离
    public float heightOffset = 0.02f; // 稍微抬高防止Z-fighting

    [Header("🚫 Obstacle Avoidance (智能避障)")]
    [Tooltip("Radius to check for obstacles. Should be slightly larger than the trap.")]
    public float checkRadius = 1.5f; // ✨ 检测半径：陷阱有多大？这里就填多大

    [Tooltip("What layers count as obstacles? (e.g., Default, Props, Walls)")]
    public LayerMask obstacleLayer; // ✨ 障碍物图层：哪些东西算障碍？

    [Tooltip("Minimum distance between traps.")]
    public float minDistanceBetweenTraps = 2.5f; // 陷阱之间的最小距离

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

        // 1. 获取生成范围 (基于平台的 Collider)
        Collider platformCol = GetComponent<Collider>();
        if (platformCol == null) { Debug.LogError("❌ No Collider on Platform!"); return; }

        // 清理旧列表
        spawnedPositions.Clear();
        // 如果是编辑器模式下反复点生成，可能需要手动清理旧生成的物体(这里略过，假设运行时生成)

        Bounds b = platformCol.bounds;
        float minX = b.min.x + edgeMargin;
        float maxX = b.max.x - edgeMargin;
        float minZ = b.min.z + edgeMargin;
        float maxZ = b.max.z - edgeMargin;
        float spawnY = b.max.y + heightOffset;

        int attempts = 0;
        int maxAttempts = trapCount * 20; // 防止死循环

        for (int i = 0; i < trapCount; i++)
        {
            bool validPositionFound = false;
            Vector3 candidatePos = Vector3.zero;

            // 尝试寻找有效位置
            while (!validPositionFound && attempts < maxAttempts)
            {
                attempts++;

                // A. 随机取点
                float rX = Random.Range(minX, maxX);
                float rZ = Random.Range(minZ, maxZ);
                candidatePos = new Vector3(rX, spawnY, rZ);

                // B. 验证位置
                if (IsPositionValid(candidatePos))
                {
                    validPositionFound = true;
                }
            }

            if (validPositionFound)
            {
                // C. 生成陷阱
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

    // ✨✨✨ 核心检测逻辑 ✨✨✨
    bool IsPositionValid(Vector3 pos)
    {
        // 1. 检查是否和其他陷阱太近
        foreach (var p in spawnedPositions)
        {
            if (Vector3.Distance(pos, p) < minDistanceBetweenTraps) return false;
        }

        // 2. 检查是否有障碍物 (Physics Overlap)
        // 我们在生成点上方一点点的位置画一个球，看有没有碰到东西
        Vector3 checkCenter = pos + Vector3.up * 1.0f; // 向上抬1米，避开地面本身

        // CheckSphere: 如果球内有 obstacleLayer 层的物体，返回 true
        if (Physics.CheckSphere(checkCenter, checkRadius, obstacleLayer))
        {
            return false; // 撞到障碍物了，位置无效
        }

        return true; // 一切正常
    }

    void OnDrawGizmos()
    {
        if (!showDebugGizmos) return;

        // 画出已生成的位置
        Gizmos.color = Color.red;
        foreach (var pos in spawnedPositions)
        {
            Gizmos.DrawWireSphere(pos + Vector3.up * 1.0f, checkRadius);
        }

        // 画出生成范围
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