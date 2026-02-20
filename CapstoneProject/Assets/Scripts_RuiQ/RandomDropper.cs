using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class MeatballDropper : MonoBehaviour
{
    [Header("🔗 Required Assets")]
    public SplineContainer targetPath;
    public GameObject meatballPrefab;

    [Header("⏲️ Spawning Logic")]
    public bool isSpawning = true;
    public float spawnInterval = 1.0f;
    [Tooltip("Every interval, should it spawn at ALL nodes or just one RANDOM node?")]
    public bool spawnAtAllNodes = false;

    [Header("📐 Range Control")]
    public float dropHeight = 15f;
    [Range(0f, 10f)]
    public float spawnRadius = 3f; // This is the "Adjustable Range" around each node

    private float timer;

    void Update()
    {
        if (targetPath == null || meatballPrefab == null || !isSpawning) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            HandleSpawning();
            timer = 0f;
        }
    }

    void HandleSpawning()
    {
        var spline = targetPath.Spline;
        var knots = spline.Knots;

        if (spawnAtAllNodes)
        {
            // Spawn one at every node near its position
            foreach (var knot in knots)
            {
                SpawnAtNode(knot);
            }
        }
        else
        {
            // Pick one random node to spawn at
            int randomIndex = UnityEngine.Random.Range(0, spline.Count);
            //SpawnAtNode(knots[randomIndex]);
            SpawnAtNode(spline[randomIndex]);
        }
    }

    void SpawnAtNode(BezierKnot knot)
    {
        // 1. Get the local position of the knot and convert to World Position
        float3 localPos = knot.Position;
        Vector3 worldNodePos = targetPath.transform.TransformPoint(localPos);

        // 2. Calculate a random offset within a circle (spawnRadius)
        Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * spawnRadius;
        Vector3 finalPos = new Vector3(
            worldNodePos.x + randomCircle.x,
            worldNodePos.y + dropHeight,
            worldNodePos.z + randomCircle.y
        );

        // 3. Instantiate the meatball
        Instantiate(meatballPrefab, finalPos, Quaternion.identity);
    }

    // Visualizes the spawn range around each node in the Editor
    private void OnDrawGizmosSelected()
    {
        if (targetPath == null) return;

        Gizmos.color = Color.cyan;
        foreach (var knot in targetPath.Spline.Knots)
        {
            Vector3 worldPos = targetPath.transform.TransformPoint(knot.Position);
            // Draw the spawn radius at each node
            Gizmos.DrawWireSphere(worldPos + Vector3.up * dropHeight, spawnRadius);
        }
    }
}