using UnityEngine;
using System.Collections;
using lilGuysNamespace;

public class BreathingWindTunnel : MonoBehaviour
{
    [Header("📍 Detection Settings")]
    public Vector3 detectionSize = new Vector3(3f, 5f, 3f);
    public Vector3 centerOffset = Vector3.zero;

    [Header("📏 Activation Conditions (Airborne Only)")]
    [Tooltip("How high off the ground to trigger? Recommended: 0.8")]
    public float activationHeight = 0.8f;
    public LayerMask groundLayer = ~0; // Default: check all layers

    [Header("🌬️ Breathing Rhythm")]
    [Tooltip("Duration of the upward blow (seconds).")]
    public float blowDuration = 2.5f;
    [Tooltip("Duration of the sinking/resting phase (seconds).")]
    public float sinkDuration = 3f;

    [Header("🚀 Flight Parameters")]
    [Tooltip("Acceleration during upward blow.\nValue of 40 ensures strong takeoff.")]
    public float blowAcceleration = 40f;

    [Tooltip("Max upward speed.\nValue of 50 prevents physics issues.")]
    public float maxBlowSpeed = 50f;

    [Tooltip("Constant sinking speed (must be negative).\nValue of -3 counteracts gravity for a slow descent.\nIn the resting phase, speed is locked to -3, never accelerating downwards.")]
    public float sinkSpeed = -3f;

    [Header("✨ VFX")]
    public ParticleSystem upParticles;
    public ParticleSystem weakParticles; // Recommended to add a weak effect for sinking phase

    // Internal State
    private enum WindState { Blowing, Sinking }
    private WindState currentState = WindState.Blowing;

    private Transform playerTransform;
    private CharacterController playerCC;
    private EntityManager playerManager;
    private bool isPlayerInside = false;
    private bool isAirborneEnough = false;

    void Start()
    {
        FindActivePlayer();
        // Remove Rigidbody if it exists to prevent conflicts
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());

        // Start the breathing cycle
        StartCoroutine(BreathCycle());
    }

    void Update()
    {
        // 🛡️ Crash prevention: stop if player is disabled or missing
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

        if (!isPlayerInside)
        {
            isAirborneEnough = false;
            return;
        }

        // 2. Airborne Detection (Smart Lock)
        // Only considered airborne if nothing is within activationHeight below feet
        bool hitGround = Physics.Raycast(playerTransform.position, Vector3.down, activationHeight, groundLayer);
        isAirborneEnough = !hitGround;

        // If grounded, return immediately. Let player walk normally. The tunnel won't interfere.
        if (!isAirborneEnough) return;

        // ================= Core Physics Logic =================

        Vector3 currentVel = playerManager.GetMovementVelocity();
        float currentY = currentVel.y;

        if (currentState == WindState.Blowing)
        {
            // --- Exhale Phase (Upward Blow) ---

            // If switching from falling state, apply strong correction to cancel falling inertia
            if (currentY < 0)
            {
                currentY = Mathf.MoveTowards(currentY, maxBlowSpeed, blowAcceleration * 2f * Time.deltaTime);
            }
            else
            {
                // Normal acceleration
                currentY = Mathf.MoveTowards(currentY, maxBlowSpeed, blowAcceleration * Time.deltaTime);
            }
        }
        else
        {
            // --- Inhale Phase (Gravity-defying Sink) ---

            // Logic: 
            // If current speed > sinkSpeed (e.g., -3), let gravity work naturally or smooth it out.
            // Once speed hits -3, lock it to create an "anti-gravity" effect.

            if (currentY > sinkSpeed)
            {
                // Still rising or falling slowly, transition smoothly to sinkSpeed
                currentY = Mathf.MoveTowards(currentY, sinkSpeed, 10f * Time.deltaTime);
            }
            else
            {
                // ⚠️ Key Point: Prevent acceleration accumulation ⚠️
                // Gravity wants to pull to -10, -20...
                // We lock it at -3. This effectively means "Wind Force = Gravity Force".
                currentY = sinkSpeed;
            }
        }

        // Apply velocity
        playerManager.SetMovementVelocity(new Vector3(currentVel.x, currentY, currentVel.z));
    }

    // 🔄 Breathing Cycle
    IEnumerator BreathCycle()
    {
        while (true)
        {
            // 1. Blowing Mode
            currentState = WindState.Blowing;
            if (upParticles) upParticles.Play();
            if (weakParticles) weakParticles.Stop();
            yield return new WaitForSeconds(blowDuration);

            // 2. Sinking Mode
            currentState = WindState.Sinking;
            if (upParticles) upParticles.Stop();
            if (weakParticles) weakParticles.Play(); // Can play a weak airflow effect here
            yield return new WaitForSeconds(sinkDuration);
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
        Gizmos.color = isPlayerInside ? Color.green : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(centerOffset, detectionSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(centerOffset, detectionSize);
    }
}