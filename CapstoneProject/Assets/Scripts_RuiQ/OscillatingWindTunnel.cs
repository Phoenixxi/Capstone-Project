using UnityEngine;
using System.Collections;
using lilGuysNamespace;

public class OscillatingWindTunnel : MonoBehaviour
{
    [Header("📍 Range Detection")]
    public Vector3 detectionSize = new Vector3(10f, 5f, 5f); // Horizontal strip
    public Vector3 centerOffset = Vector3.zero;

    [Header("🌊 Sine Wave Settings")]
    [Tooltip("Check this! (Checked by default)")]
    public bool useSineWaveMode = true;

    [Tooltip("How many seconds for a full cycle?\n(Left->Right->Left)\nRecommended: 6 to 10.")]
    public float waveCycleTime = 8f;

    [Header("🚀 Wind Parameters")]
    [Tooltip("Wind acceleration (Push force).\nSince sine wave has gradation, recommend higher values like 30-40.")]
    public float windAcceleration = 35f;

    [Tooltip("Max wind speed (Limit)")]
    public float maxWindSpeed = 15f;

    [Header("✨ VFX (Required)")]
    [Tooltip("Particles playing when blowing Left")]
    public ParticleSystem leftParticles;
    [Tooltip("Particles playing when blowing Right")]
    public ParticleSystem rightParticles;

    // Internal Variables
    private float currentWindStrength = 0f; // -1 (Left) to 1 (Right)
    private Transform playerTransform;
    private CharacterController playerCC;
    private EntityManager playerManager;
    private bool isPlayerInside = false;

    void Start()
    {
        FindActivePlayer();
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());

        // Start different modes based on selection
        if (useSineWaveMode)
            StartCoroutine(SineWaveCycle());
        else
            StartCoroutine(HardSwitchCycle());
    }

    void Update()
    {
        // 🛡️ Crash prevention
        if (playerCC == null || playerManager == null || !playerCC.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
            if (playerCC == null) return;
        }
        if (!playerCC.enabled) return;

        // 1. Range Detection
        Vector3 localPos = transform.InverseTransformPoint(playerTransform.position);
        localPos -= centerOffset;
        bool insideX = Mathf.Abs(localPos.x) <= detectionSize.x * 0.5f;
        bool insideY = Mathf.Abs(localPos.y) <= detectionSize.y * 0.5f;
        bool insideZ = Mathf.Abs(localPos.z) <= detectionSize.z * 0.5f;

        isPlayerInside = (insideX && insideY && insideZ);

        if (!isPlayerInside) return;

        // ================= 🌪️ Apply Wind Force (Original Logic) =================

        // Dead zone: If wind is too weak (< 0.1), treat as no wind to let player rest
        if (Mathf.Abs(currentWindStrength) < 0.1f) return;

        Vector3 currentVel = playerManager.GetMovementVelocity();

        // Determine direction: transform.right is the red axis
        // Positive strength -> Right; Negative -> Left
        Vector3 targetDirection = transform.right * Mathf.Sign(currentWindStrength);

        // Calculate actual force: Strength * Acceleration
        // Sine wave peaks at 1, valleys at 0, creating a natural "fade in/out" feel
        float finalAccel = windAcceleration * Mathf.Abs(currentWindStrength);

        Vector3 windForce = targetDirection * finalAccel * Time.deltaTime;
        Vector3 newVel = currentVel + windForce;

        // Speed limit (Lateral only)
        float speedInWindDir = Vector3.Dot(newVel, targetDirection);
        if (speedInWindDir > maxWindSpeed)
        {
            Vector3 correction = targetDirection * (speedInWindDir - maxWindSpeed);
            newVel -= correction;
        }

        playerManager.SetMovementVelocity(newVel);
    }

    // 🌊 Sine Wave Cycle (Smooth Transition)
    IEnumerator SineWaveCycle()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            // Core formula: Generate smooth wave from -1 to 1
            // Use Mathf.Sin for natural fluctuation
            float wave = Mathf.Sin((timer / waveCycleTime) * Mathf.PI * 2);

            currentWindStrength = wave;

            // Particle Control (Smooth Toggle)
            // Enable Left particles when blowing left strength > 0.2
            if (wave < -0.2f)
            {
                if (leftParticles && !leftParticles.isPlaying) leftParticles.Play();
                if (rightParticles) rightParticles.Stop();
            }
            // Enable Right particles when blowing right strength > 0.2
            else if (wave > 0.2f)
            {
                if (leftParticles) leftParticles.Stop();
                if (rightParticles && !rightParticles.isPlaying) rightParticles.Play();
            }
            // Weak transition period (Calm)
            else
            {
                if (leftParticles) leftParticles.Stop();
                if (rightParticles) rightParticles.Stop();
            }

            yield return null; // Wait for next frame
        }
    }

    // Backup: Hard Switch Mode
    IEnumerator HardSwitchCycle()
    {
        // (Uncheck in Inspector to switch to this mode)
        float blowDuration = 3f;
        float pauseDuration = 2f;
        while (true)
        {
            currentWindStrength = -1f; // Left
            if (leftParticles) leftParticles.Play(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(blowDuration);

            currentWindStrength = 0f; // Stop
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(pauseDuration);

            currentWindStrength = 1f; // Right
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Play();
            yield return new WaitForSeconds(blowDuration);

            currentWindStrength = 0f; // Stop
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(pauseDuration);
        }
    }

    void FindActivePlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null && playerObj.activeInHierarchy)
        {
            playerTransform = playerObj.transform;
            playerCC = playerObj.GetComponent<CharacterController>();
            playerManager = playerObj.GetComponent<EntityManager>();
            if (playerCC == null) playerCC = playerObj.GetComponentInChildren<CharacterController>();
            if (playerManager == null) playerManager = playerObj.GetComponentInChildren<EntityManager>();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawCube(centerOffset, detectionSize);
        Gizmos.DrawWireCube(centerOffset, detectionSize);

        // Dynamic Arrow: Changes based on wind force and direction
        Vector3 right = Vector3.right * (detectionSize.x * 0.4f);
        if (Mathf.Abs(currentWindStrength) > 0.1f)
        {
            Gizmos.color = currentWindStrength > 0 ? Color.red : Color.blue;
            Gizmos.DrawRay(centerOffset, right * currentWindStrength);
            Gizmos.DrawSphere(centerOffset + right * currentWindStrength, 0.4f);
        }
    }
}