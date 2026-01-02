using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using lilGuysNamespace;

public class ManEatingHole : MonoBehaviour
{
    private enum HoleState { Idle, Swallowing, Chewing, Spitting, Cooldown }
    private HoleState currentState = HoleState.Idle;

    [Header("👇 Generate / Update")]
    public bool clickToGenerate = false;

    [Header("🚫 Freeze Settings")]
    public SwappingManager swappingManager;

    [Header("🎨 Art Assets")]
    public GameObject petalPrototype;
    public Material holeBlackMaterial;

    [Header("📏 Generation Settings")]
    public int petalCount = 8;
    public float ringRadius = 2f;
    public float holeCenterSize = 2f;

    [Header("↕️ Height Adjustment")]
    public float petalHeightOffset = 0f;
    public float holeThickness = 0.01f;
    public float holeVerticalOffset = 0.02f;

    [Header("🌈 Color Indicator")]
    [ColorUsage(true, true)]
    public Color idleColor = new Color(0, 1, 0.5f);
    [ColorUsage(true, true)]
    public Color attackColor = Color.red;
    [ColorUsage(true, true)]
    public Color cooldownColor = Color.gray;

    [Header("⚙️ Motion Settings")]
    public bool reverseAnimation = false;
    public Vector3 closeRotationAxis = new Vector3(0, 1, 0);
    public float openAngle = 0f;
    public float closedAngle = 90f;
    public float detectionRange = 5f;
    public float smoothSpeed = 5f;

    [Header("🦷 Trap Settings")]
    public float triggerRadius = 0.4f;
    public float chewDuration = 3f;

    [Header("🚀 Spit Settings (Manual Physics)")]
    public float spitForce = 20f;
    public float gravityScale = 2.0f;
    [Range(0, 1)] public float spitRandomness = 0.3f;
    public float eatCooldown = 5f;

    [Header("🩸 VFX References")]
    public ParticleSystem bloodEffect;
    public ParticleSystem spitEffect;

    // --- Internal Variables ---
    private Transform playerRoot;
    private EntityManager currentPlayerEntity;
    private Collider currentPlayerCollider;
    private Rigidbody currentPlayerRb;
    private CharacterController currentPlayerCC;

    private List<Transform> activePetals = new List<Transform>();
    private List<Quaternion> initialRotations = new List<Quaternion>();
    private List<Renderer> petalRenderers = new List<Renderer>();
    private float currentTargetAngle = 0f;
    private Color currentTargetColor;
    private Vector3 originalPlayerScale;

    void OnValidate()
    {
        if (clickToGenerate) { GenerateEverything(); clickToGenerate = false; }
        UpdateHoleMaterial();
    }

    void Start()
    {
        if (swappingManager == null) swappingManager = FindAnyObjectByType<SwappingManager>();
        FindPlayerRoot();
        InitializePetals();

        currentTargetColor = idleColor;
        if (activePetals.Count == 0 && petalPrototype != null) GenerateEverything();
        UpdateHoleMaterial();
    }

    void Update()
    {
        if (currentState == HoleState.Idle || currentState == HoleState.Cooldown) FindPlayerRoot();
        if (playerRoot == null) return;

        switch (currentState)
        {
            case HoleState.Idle:
                HandleIdleState();
                currentTargetColor = idleColor;
                break;
            case HoleState.Swallowing:
                currentTargetAngle = closedAngle;
                currentTargetColor = attackColor;
                break;
            case HoleState.Chewing:
                float chewOffset = Mathf.PingPong(Time.time * 20, 10);
                currentTargetAngle = closedAngle + chewOffset;
                currentTargetColor = Color.Lerp(attackColor, Color.black, Mathf.PingPong(Time.time * 5, 1));
                break;
            case HoleState.Spitting:
                currentTargetAngle = openAngle;
                currentTargetColor = cooldownColor;
                break;
            case HoleState.Cooldown:
                HandleDistanceOpen();
                currentTargetColor = cooldownColor;
                break;
        }

        UpdatePetalRotation();
        UpdatePetalColor();
    }

    void UpdateHoleMaterial()
    {
        Transform fakeHole = transform.Find("Fake_Black_Hole");
        if (fakeHole != null && holeBlackMaterial != null)
        {
            Renderer r = fakeHole.GetComponent<Renderer>();
            if (r != null) r.sharedMaterial = holeBlackMaterial;
        }
    }

    void HandleIdleState()
    {
        if (playerRoot == null) return;
        Vector3 flatPlayer = new Vector3(playerRoot.position.x, 0, playerRoot.position.z);
        Vector3 flatHole = new Vector3(transform.position.x, 0, transform.position.z);

        if (Vector3.Distance(flatPlayer, flatHole) < triggerRadius)
        {
            StartCoroutine(DigestionRoutine());
        }

        HandleDistanceOpen();
    }

    IEnumerator DigestionRoutine()
    {
        currentState = HoleState.Swallowing;
        smoothSpeed = 15f;

        // 1. Get Components
        if (playerRoot != null)
        {
            currentPlayerEntity = playerRoot.GetComponent<EntityManager>();
            if (currentPlayerEntity == null) currentPlayerEntity = playerRoot.GetComponentInChildren<EntityManager>();
            currentPlayerCollider = playerRoot.GetComponent<Collider>();
            currentPlayerRb = playerRoot.GetComponent<Rigidbody>();
            currentPlayerCC = playerRoot.GetComponent<CharacterController>();
        }

        // 2. Freeze Player
        if (swappingManager != null) swappingManager.enabled = false;
        if (currentPlayerEntity != null) currentPlayerEntity.enabled = false;
        if (currentPlayerCC != null) currentPlayerCC.enabled = false;
        if (currentPlayerRb != null) { currentPlayerRb.linearVelocity = Vector3.zero; currentPlayerRb.isKinematic = true; }

        if (currentPlayerRb != null) currentPlayerCollider.enabled = false;

        originalPlayerScale = playerRoot.localScale;
        playerRoot.localScale = Vector3.zero;
        playerRoot.position = transform.position;

        yield return new WaitForSeconds(0.5f);

        // 3. Chew
        currentState = HoleState.Chewing;
        float timer = 0f;
        while (timer < chewDuration)
        {
            transform.localScale = Vector3.one * Random.Range(0.9f, 1.1f);
            if (bloodEffect) bloodEffect.Play();
            yield return new WaitForSeconds(0.2f);
            timer += 0.2f;
        }
        transform.localScale = Vector3.one;

        // 4. Spit (Manual Physics)
        currentState = HoleState.Spitting;
        smoothSpeed = 20f;
        if (spitEffect) spitEffect.Play();

        playerRoot.localScale = originalPlayerScale;

        // A. Teleport up
        playerRoot.position = transform.position + Vector3.up * 2.0f;

        // B. Calculate Initial Velocity
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized * spitRandomness;
        Vector3 currentVelocity = (Vector3.up + randomDir).normalized * spitForce;

        // C. Manual Physics Loop
        float safetyTimer = 0f;
        bool hasLanded = false;

        // Enable CC
        if (currentPlayerCC != null)
        {
            currentPlayerCC.enabled = true;
        }

        // ✨✨✨ CRITICAL FIX: Wait 1 frame for Unity to activate CC ✨✨✨
        yield return null;

        while (!hasLanded && safetyTimer < 3.0f)
        {
            float dt = Time.deltaTime;
            safetyTimer += dt;

            // Apply Gravity
            currentVelocity += Physics.gravity * gravityScale * dt;

            // Move
            Vector3 moveDelta = currentVelocity * dt;

            if (currentPlayerCC != null && currentPlayerCC.enabled)
            {
                currentPlayerCC.Move(moveDelta);
                hasLanded = currentPlayerCC.isGrounded;
            }
            else
            {
                // Fallback for non-CC characters
                playerRoot.position += moveDelta;

                if (currentVelocity.y < 0)
                {
                    if (Physics.Raycast(playerRoot.position + Vector3.up, Vector3.down, 1.2f))
                    {
                        hasLanded = true;
                    }
                }
            }

            // Floor check to prevent falling through map
            if (playerRoot.position.y < transform.position.y - 0.5f)
            {
                hasLanded = true;
            }

            yield return null;
        }

        // 5. Land & Recover
        if (currentPlayerRb != null)
        {
            currentPlayerRb.isKinematic = false;
            currentPlayerRb.linearVelocity = Vector3.zero;
            if (currentPlayerCollider != null) currentPlayerCollider.enabled = true;
        }

        yield return null;

        if (currentPlayerEntity != null) currentPlayerEntity.enabled = true;
        if (swappingManager != null) swappingManager.enabled = true;

        yield return new WaitForSeconds(0.3f);
        currentState = HoleState.Cooldown;
        smoothSpeed = 5f;
        yield return new WaitForSeconds(eatCooldown);
        currentState = HoleState.Idle;
    }

    void FindPlayerRoot()
    {
        Transform target = null;
        if (swappingManager != null) target = swappingManager.GetCurrentCharacterTransform();
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) target = playerObj.transform;
        }

        if (target != null)
        {
            CharacterController rootCC = target.GetComponentInParent<CharacterController>();
            if (rootCC != null) { playerRoot = rootCC.transform; return; }

            Rigidbody rootRb = target.GetComponentInParent<Rigidbody>();
            if (rootRb != null) { playerRoot = rootRb.transform; return; }

            EntityManager rootEntity = target.GetComponentInParent<EntityManager>();
            if (rootEntity != null) { playerRoot = rootEntity.transform; return; }

            playerRoot = target;
        }
    }

    public void GenerateEverything()
    {
        if (petalPrototype == null) { Debug.LogError("❌ Missing Prototype!"); return; }
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (Transform child in transform) if (child.gameObject != petalPrototype) toDestroy.Add(child.gameObject);
        foreach (GameObject obj in toDestroy) { if (Application.isPlaying) Destroy(obj); else DestroyImmediate(obj); }

        activePetals.Clear();
        petalPrototype.SetActive(false);
        float angleStep = 360f / petalCount;
        for (int i = 0; i < petalCount; i++)
        {
            GameObject pivotGO = new GameObject($"Petal_Pivot_{i}");
            pivotGO.transform.SetParent(this.transform);
            pivotGO.transform.localPosition = Vector3.zero;
            pivotGO.transform.localRotation = Quaternion.Euler(0, i * angleStep, 0);
            GameObject clone = Instantiate(petalPrototype, pivotGO.transform);
            clone.name = $"Mesh_{i}";
            clone.SetActive(true);
            clone.transform.localPosition = new Vector3(0, petalHeightOffset, ringRadius);
            clone.transform.localRotation = Quaternion.identity;
        }

        GameObject fakeHole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fakeHole.name = "Fake_Black_Hole";
        fakeHole.transform.SetParent(this.transform);
        fakeHole.transform.localPosition = new Vector3(0, holeVerticalOffset, 0);
        fakeHole.transform.localRotation = Quaternion.identity;
        fakeHole.transform.localScale = new Vector3(holeCenterSize, holeThickness, holeCenterSize);
        DestroyImmediate(fakeHole.GetComponent<Collider>());

        if (holeBlackMaterial != null)
        {
            Renderer r = fakeHole.GetComponent<Renderer>();
            r.sharedMaterial = holeBlackMaterial;
        }
        InitializePetals();
    }

    void InitializePetals()
    {
        activePetals.Clear(); initialRotations.Clear(); petalRenderers.Clear();
        foreach (Transform pivot in transform)
        {
            if (!pivot.gameObject.activeSelf || pivot.name.StartsWith("Fake_Black")) continue;
            activePetals.Add(pivot); initialRotations.Add(pivot.localRotation);
            Renderer r = pivot.GetComponentInChildren<Renderer>();
            if (r != null) petalRenderers.Add(r);
        }
    }
    void UpdatePetalColor()
    {
        foreach (var r in petalRenderers) { if (r == null) continue; r.material.color = Color.Lerp(r.material.color, currentTargetColor, Time.deltaTime * 5f); }
    }
    void UpdatePetalRotation()
    {
        for (int i = 0; i < activePetals.Count; i++)
        {
            Quaternion targetRotation = initialRotations[i] * Quaternion.AngleAxis(currentTargetAngle, closeRotationAxis);
            activePetals[i].localRotation = Quaternion.Slerp(activePetals[i].localRotation, targetRotation, Time.deltaTime * smoothSpeed);
        }
    }
    void HandleDistanceOpen()
    {
        if (playerRoot == null) return;
        Vector3 flatPlayerPos = new Vector3(playerRoot.position.x, transform.position.y, playerRoot.position.z);
        float distance = Vector3.Distance(transform.position, flatPlayerPos);
        float rawFactor = Mathf.InverseLerp(1.5f, detectionRange, distance);
        float openFactor = reverseAnimation ? rawFactor : (1f - rawFactor);
        currentTargetAngle = Mathf.Lerp(closedAngle, openAngle, Mathf.Clamp01(openFactor));
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 1, 0, 0.3f); Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = new Color(1, 0, 0, 0.4f); Gizmos.DrawSphere(transform.position, triggerRadius);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, triggerRadius);
#if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, "🦟 Man-Eater");
#endif
    }
}