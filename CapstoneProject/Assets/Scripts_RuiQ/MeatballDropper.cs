using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MeatballDropper : MonoBehaviour
{
    [Header("🔗 Required Assets")]
    public SplineContainer targetPath;
    public GameObject meatballPrefab;

    [Header("⏲️ Spawning Logic")]
    public bool isSpawning = false;
    [Tooltip("新节点的默认生成频率")]
    public float defaultInterval = 1.0f;

    [Header("📐 Cylinder Settings")]
    public float dropHeight = 15f;
    [Tooltip("圆柱体的高度范围（纵向随机区间）")]
    public float cylinderHeight = 5f;
    public float defaultRadius = 3f;

    [Header("🎯 Physics Control")]
    [Tooltip("限制肉丸下落的最大速度")]
    public float maxFallSpeed = 25f;

    [Header("🎯 Individual Knot Settings")]
    [Tooltip("手动调节每个节点的半径")]
    public List<float> nodeRadii = new List<float>();
    [Tooltip("手动调节每个节点的生成频率（秒/个）")]
    public List<float> nodeIntervals = new List<float>();

    // 为每个节点存储独立的计时器
    private List<float> nodeTimers = new List<float>();

    private void OnValidate()
    {
        if (targetPath == null || targetPath.Spline == null) return;
        int knotCount = targetPath.Spline.Count;

        // 自动对齐半径列表
        SyncList(nodeRadii, knotCount, defaultRadius);
        // 自动对齐频率列表
        SyncList(nodeIntervals, knotCount, defaultInterval);
    }

    private void SyncList(List<float> list, int count, float defaultValue)
    {
        while (list.Count < count) list.Add(defaultValue);
        while (list.Count > count) list.RemoveAt(list.Count - 1);
    }

    void Update()
    {
        if (targetPath == null || meatballPrefab == null || !isSpawning || targetPath.Spline.Count == 0) return;

        // 初始化或更新内部计时器列表长度
        while (nodeTimers.Count < targetPath.Spline.Count) nodeTimers.Add(0f);

        // 核心：遍历所有节点，每个节点运行自己的时钟
        for (int i = 0; i < targetPath.Spline.Count; i++)
        {
            nodeTimers[i] += Time.deltaTime;

            // 如果当前节点的时间到了，就生成
            if (nodeTimers[i] >= nodeIntervals[i])
            {
                SpawnAtNode(targetPath.Spline[i], i);
                nodeTimers[i] = 0f; // 重置该节点的计时器
            }
        }
    }

    void SpawnAtNode(BezierKnot knot, int index)
    {
        Vector3 worldNodePos = targetPath.transform.TransformPoint((float3)knot.Position);
        float radius = nodeRadii[index];

        // 1. 水平随机 (圆柱截面)
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;

        // 2. 垂直随机 (圆柱高度)
        float randomYOffset = UnityEngine.Random.Range(0, cylinderHeight);

        Vector3 finalPos = new Vector3(
            worldNodePos.x + randomCircle.x,
            worldNodePos.y + dropHeight + randomYOffset,
            worldNodePos.z + randomCircle.y
        );

        GameObject ball = Instantiate(meatballPrefab, finalPos, Quaternion.identity);

        // 3. 物理限速传递
        Meatball mb = ball.GetComponent<Meatball>();
        if (mb != null) mb.maxSpeed = maxFallSpeed;
    }

    public void SetSpawning(bool state) => isSpawning = state;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (targetPath == null || targetPath.Spline == null || nodeRadii.Count != targetPath.Spline.Count) return;

        for (int i = 0; i < targetPath.Spline.Count; i++)
        {
            var knot = targetPath.Spline[i];
            Vector3 worldPos = targetPath.transform.TransformPoint((float3)knot.Position);
            float radius = nodeRadii[i];
            float interval = nodeIntervals[i];

            // 可视化圆柱体
            Handles.color = new Color(1, 0.5f, 0, 0.3f);
            Vector3 bottom = worldPos + Vector3.up * dropHeight;
            Vector3 top = bottom + Vector3.up * cylinderHeight;
            
            Handles.DrawWireDisc(bottom, Vector3.up, radius);
            Handles.DrawWireDisc(top, Vector3.up, radius);
            Handles.DrawLine(bottom + Vector3.left * radius, top + Vector3.left * radius);

            // 显示信息标签：增加了间隔时间显示
            GUIStyle style = new GUIStyle { normal = { textColor = Color.white }, fontSize = 11, fontStyle = FontStyle.Bold };
            Handles.Label(top + Vector3.up * 0.5f, $"[{i}] R:{radius} | Int:{interval}s", style);
        }
    }
#endif
}