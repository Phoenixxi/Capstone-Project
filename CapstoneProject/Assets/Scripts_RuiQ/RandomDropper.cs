using UnityEngine;

public class RandomDropper : MonoBehaviour
{
    [Header("🔧 Spawning Settings")]
    [Tooltip("The prefab of the ball/hazard to spawn.")]
    public GameObject ballPrefab;

    [Tooltip("The size of the area (Length and Width).")]
    public Vector2 spawnAreaSize = new Vector2(20f, 5f);

    [Tooltip("How high above this object the ball spawns.")]
    public float dropHeight = -1.0f;

    [Tooltip("Time interval between spawns (in seconds).")]
    public float spawnInterval = 0.5f; // Faster spawn rate recommended for gradients

    [Header("📈 Density Control")]
    [Tooltip("Controls where balls spawn most often. X-axis (0 to 1) is Random Input, Y-axis (0 to 1) is Position Left-to-Right.")]
    public AnimationCurve spawnDistribution = AnimationCurve.Linear(0, 0, 1, 1);

    private float timer;

    void Start()
    {
        if (ballPrefab == null) Debug.LogError("❌ Prefab missing!");
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBall();
            timer = 0f;
        }
    }

    void SpawnBall()
    {
        if (ballPrefab == null) return;

        // 1. Get a random value between 0 and 1
        float randomRoll = Random.value;

        // 2. Evaluate the curve to get the "weighted" position
        // If the curve bows UP, more balls appear on the Right (High values).
        // If the curve bows DOWN, more balls appear on the Left (Low values).
        float weightedT = spawnDistribution.Evaluate(randomRoll);

        // 3. Map the 0-1 value to the actual world coordinates (-size/2 to +size/2)
        // Lerp Unclamped allows going slightly outside if curve goes above 1, but usually 0-1 is best.
        float randomX = Mathf.Lerp(-spawnAreaSize.x / 2, spawnAreaSize.x / 2, weightedT);

        // Z is still random uniform (optional: can use another curve for Z if needed)
        float randomZ = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);

        Vector3 spawnPos = new Vector3(transform.position.x + randomX, transform.position.y + dropHeight, transform.position.z + randomZ);

        Instantiate(ballPrefab, spawnPos, Quaternion.identity);
    }

    // Visualization: Shows the density gradient using lines
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(transform.position.x, transform.position.y + dropHeight, transform.position.z);
        Gizmos.DrawWireCube(center, new Vector3(spawnAreaSize.x, 1, spawnAreaSize.y));

        // Visualize the "Heavy" side with red lines
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        for (int i = 0; i < 20; i++)
        {
            float t = spawnDistribution.Evaluate(i / 19f);
            float x = Mathf.Lerp(-spawnAreaSize.x / 2, spawnAreaSize.x / 2, t);
            Vector3 linePos = new Vector3(transform.position.x + x, transform.position.y + dropHeight, transform.position.z);
            Gizmos.DrawLine(linePos + Vector3.back * 2, linePos + Vector3.forward * 2);
        }
    }
}