using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoColorPortal : MonoBehaviour
{
    public enum PortalChannel
    {
        Red, Blue, Green, Yellow, Purple, Cyan
    }

    [Header("📢 Settings")]
    [Tooltip("Choose a color; the script will automatically link to another portal in the scene with the same color.")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("🎨 Visuals")]
    [Tooltip("Drag the disc model here to automatically apply the channel color.")]
    public MeshRenderer portalRenderer;

    [Header("⏱️ Animation Settings")]
    [Tooltip("Duration for the scale-up/down animation (seconds).")]
    public float animationDuration = 0.5f;

    [Header("🚀 Side-Scrolling Toss Settings")]
    [Tooltip("Horizontal distance to throw the player (X-axis).")]
    public float throwDistance = 5f;
    [Tooltip("Vertical height of the throw arc (Y-axis).")]
    public float throwHeight = 3f;
    [Tooltip("Duration of the flight (seconds).")]
    public float flightDuration = 0.8f;

    // --- Internal Variables ---
    private AutoColorPortal linkedTarget;
    private bool isCoolingDown = false;
    private Collider myCollider;

    // Color mapping dictionary
    private readonly Dictionary<PortalChannel, Color> channelColors = new Dictionary<PortalChannel, Color>()
    {
        { PortalChannel.Red, Color.red },
        { PortalChannel.Blue, Color.blue },
        { PortalChannel.Green, Color.green },
        { PortalChannel.Yellow, Color.yellow },
        { PortalChannel.Purple, new Color(0.5f, 0f, 0.5f) },
        { PortalChannel.Cyan, Color.cyan }
    };

    void Awake()
    {
        myCollider = GetComponent<Collider>();
        FindMyPartner();
        UpdateVisualColor();
    }

    void OnValidate()
    {
        UpdateVisualColor();
    }

    public void FindMyPartner()
    {
        var allPortals = FindObjectsByType<AutoColorPortal>(FindObjectsSortMode.None);
        foreach (var portal in allPortals)
        {
            // Search for another portal instance with the matching color channel
            if (portal != this && portal.portalColor == this.portalColor)
            {
                linkedTarget = portal;
                return;
            }
        }
        linkedTarget = null;
    }

    void UpdateVisualColor()
    {
        if (portalRenderer != null && channelColors.ContainsKey(portalColor))
        {
            // Apply color to the material's color property (handles both URP and Standard shaders)
            Material mat = new Material(portalRenderer.sharedMaterial);
            Color targetColor = channelColors[portalColor];

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", targetColor);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", targetColor);

            portalRenderer.material = mat;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (linkedTarget == null) FindMyPartner();

        if (other.CompareTag("Player") && !isCoolingDown && linkedTarget != null)
        {
            StartCoroutine(TeleportProcess(other.gameObject));
        }
    }

    private IEnumerator TeleportProcess(GameObject player)
    {
        // 1. Disable player controls and physics
        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (cc) cc.enabled = false;
        if (rb) rb.isKinematic = true;

        Vector3 startScale = player.transform.localScale;
        Vector3 startPos = player.transform.position;
        float timer = 0f;

        // --- Phase 1: Inhale and shrink ---
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            player.transform.position = Vector3.Lerp(startPos, transform.position, t);
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        player.transform.localScale = Vector3.zero;

        // --- Phase 2: Teleportation ---
        linkedTarget.StartCooldown();
        player.transform.position = linkedTarget.transform.position;

        // --- Phase 3: 🔥 Get exit direction 🔥 ---
        // Read the target portal's transform.right to determine ejection direction
        Vector3 portalRight = linkedTarget.transform.right;

        // Normalize to pure Left (-1) or Right (1) to prevent diagonal drift if the portal is tilted
        Vector3 throwDir = (portalRight.x >= 0) ? Vector3.right : Vector3.left;

        // --- Phase 4: Scale back up + Ejection arc ---
        Coroutine throwRoutine = StartCoroutine(ThrowPlayerSideScroll(player, linkedTarget, throwDir));

        timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            player.transform.localScale = Vector3.Lerp(Vector3.zero, startScale, t);
            yield return null;
        }
        player.transform.localScale = startScale;

        yield return throwRoutine;

        // --- Phase 5: Restore controls ---
        if (cc) cc.enabled = true;
        if (rb) rb.isKinematic = false;
    }

    private IEnumerator ThrowPlayerSideScroll(GameObject player, AutoColorPortal originPortal, Vector3 direction)
    {
        float elapsed = 0f;
        Vector3 p0 = originPortal.transform.position;

        // Define landing point
        Vector3 p2 = p0 + (direction * throwDistance);

        // Define control point (apex of the jump arc)
        Vector3 p1 = p0 + (direction * (throwDistance / 2)) + (Vector3.up * throwHeight);

        // Lock the Z-axis for 2.5D side-scrolling consistency
        float fixedZ = player.transform.position.z;

        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flightDuration;

            // Quadratic Bezier curve calculation
            Vector3 position = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;
            position.z = fixedZ;

            player.transform.position = position;
            yield return null;
        }

        Vector3 finalPos = p2;
        finalPos.z = fixedZ;
        player.transform.position = finalPos;
    }

    public void StartCooldown()
    {
        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isCoolingDown = true;
        if (myCollider) myCollider.enabled = false;
        yield return new WaitForSeconds(1.0f);
        isCoolingDown = false;
        if (myCollider) myCollider.enabled = true;
    }

    void OnDrawGizmos()
    {
        if (linkedTarget != null)
        {
            // Draw a line connecting linked portals in the Editor for debugging
            Gizmos.color = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;
            Gizmos.DrawLine(transform.position, linkedTarget.transform.position);

            // Draw the ejection direction ray (Yellow)
            Vector3 dir = transform.right.x >= 0 ? Vector3.right : Vector3.left;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, dir * 2f);
        }
    }
}