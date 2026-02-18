using UnityEngine;
using UnityEngine.Splines; // Required: Install Splines package

public class SimpleSplineRangeDropper : MonoBehaviour
{
    [Header("🔧 Required References")]
    public SplineContainer path;
    public GameObject meatballPrefab;

    [Header("📈 Spawn Range Settings")]
    public float spawnInterval = 0.5f;
    public float dropHeight = 10f;

    // The width of the "spawn ribbon" (left and right from the spline)
    public float horizontalRange = 3f;

    // The thickness of the "spawn ribbon" (forward and backward along the spline)
    public float forwardRange = 1f;

    private float timer;

    void Update()
    {
        if (path == null || meatballPrefab == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnWithRange();
            timer = 0f;
        }
    }

    void SpawnWithRange()
    {
        // 1. Pick a random point 't' on the spline (0 to 1)
        float t = Random.value;

        // 2. Get the base position and the forward direction (tangent) at that point
        Vector3 basePos = path.EvaluatePosition(t);
        Vector3 forward = ((Vector3)path.EvaluateTangent(t)).normalized;

        // 3. Calculate the 'right' vector (perpendicular to the path)
        // Using Vector3.up ensures the 'right' is always horizontal
        Vector3 right = Vector3.Cross(Vector3.up, forward).normalized;

        // 4. Generate random offsets within the defined range
        float randomRight = Random.Range(-horizontalRange, horizontalRange);
        float randomForward = Random.Range(-forwardRange, forwardRange);

        // 5. Combine everything to get the final spawn position
        // Base point + height + side offset + forward offset
        Vector3 finalSpawnPos = basePos
                                + (Vector3.up * dropHeight)
                                + (right * randomRight)
                                + (forward * randomForward);

        Instantiate(meatballPrefab, finalSpawnPos, Quaternion.identity);
    }
}