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
    [Tooltip("Default spawn interval for new knots")]
    public float defaultInterval = 1.0f;

    [Header("📐 Cylinder Settings")]
    public float dropHeight = 15f;
    [Tooltip("Height range of the cylinder (vertical random range)")]
    public float cylinderHeight = 5f;
    public float defaultRadius = 3f;

    [Header("🎯 Physics Control")]
    [Tooltip("Maximum falling speed limit for the meatball")]
    public float maxFallSpeed = 25f;

    [Header("🎯 Individual Knot Settings")]
    [Tooltip("Manual radius adjustment for each knot")]
    public List<float> nodeRadii = new List<float>();
    [Tooltip("Manual spawn interval for each knot (seconds per spawn)")]
    public List<float> nodeIntervals = new List<float>();

    // Store independent timers for each knot
    private List<float> nodeTimers = new List<float>();

    private void OnValidate()
    {
        if (targetPath == null || targetPath.Spline == null) return;
        int knotCount = targetPath.Spline.Count;

        // Sync the radius list to match the knot count
        SyncList(nodeRadii, knotCount, defaultRadius);
        // Sync the interval list to match the knot count
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

        // Initialize or update the internal timer list length
        while (nodeTimers.Count < targetPath.Spline.Count) nodeTimers.Add(0f);

        // Core logic: Iterate through all knots, each running its own timer
        for (int i = 0; i < targetPath.Spline.Count; i++)
        {
            nodeTimers[i] += Time.deltaTime;

            // Spawn if the current knot's timer has been reached
            if (nodeTimers[i] >= nodeIntervals[i])
            {
                SpawnAtNode(targetPath.Spline[i], i);
                nodeTimers[i] = 0f; // Reset the timer for this knot
            }
        }
    }

    void SpawnAtNode(BezierKnot knot, int index)
    {
        Vector3 worldNodePos = targetPath.transform.TransformPoint((float3)knot.Position);
        float radius = nodeRadii[index];

        // 1. Horizontal randomness (cylinder cross-section)
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * radius;

        // 2. Vertical randomness (cylinder height)
        float randomYOffset = UnityEngine.Random.Range(0, cylinderHeight);

        Vector3 finalPos = new Vector3(
            worldNodePos.x + randomCircle.x,
            worldNodePos.y + dropHeight + randomYOffset,
            worldNodePos.z + randomCircle.y
        );

        GameObject ball = Instantiate(meatballPrefab, finalPos, Quaternion.identity);

        // 3. Apply physics speed limit
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

            // Visualize the cylinder
            Handles.color = new Color(1, 0.5f, 0, 0.3f);
            Vector3 bottom = worldPos + Vector3.up * dropHeight;
            Vector3 top = bottom + Vector3.up * cylinderHeight;
            
            Handles.DrawWireDisc(bottom, Vector3.up, radius);
            Handles.DrawWireDisc(top, Vector3.up, radius);
            Handles.DrawLine(bottom + Vector3.left * radius, top + Vector3.left * radius);

            // Display info label: Added interval time display
            GUIStyle style = new GUIStyle { normal = { textColor = Color.white }, fontSize = 11, fontStyle = FontStyle.Bold };
            Handles.Label(top + Vector3.up * 0.5f, $"[{i}] R:{radius} | Int:{interval}s", style);
        }
    }
#endif
}