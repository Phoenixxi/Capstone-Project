using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoColorPortal : MonoBehaviour
{
    public enum PortalChannel
    {
        Red, Blue, Green, Yellow, Purple, Cyan
    }

    [Header("📢 Settings")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("🎨 Visuals")]
    [Tooltip("The portal disc mesh renderer.")]
    public MeshRenderer portalRenderer;
    [Tooltip("VFX prefab to spawn on teleport.")]
    public GameObject teleportVFX;

    [Header("⏱️ Animation Settings")]
    public float animationDuration = 0.4f;

    // --- Internal Variables ---
    private AutoColorPortal linkedTarget;
    private bool isCoolingDown = false;
    private BoxCollider myCollider;

    private readonly Dictionary<PortalChannel, Color> channelColors = new Dictionary<PortalChannel, Color>()
    {
        { PortalChannel.Red, Color.red },
        { PortalChannel.Blue, Color.blue },
        { PortalChannel.Green, Color.green },
        { PortalChannel.Yellow, Color.yellow },
        { PortalChannel.Purple, new Color(0.6f, 0f, 1f) },
        { PortalChannel.Cyan, Color.cyan }
    };

    void Awake()
    {
        myCollider = GetComponent<BoxCollider>();
        FindMyPartner();
        UpdateVisualColor();
    }

    void OnValidate()
    {
        UpdateVisualColor();
    }

    public void FindMyPartner()
    {
        var allPortals = Object.FindObjectsByType<AutoColorPortal>(FindObjectsSortMode.None);
        foreach (var portal in allPortals)
        {
            if (portal != this && portal.portalColor == this.portalColor)
            {
                linkedTarget = portal;
                return;
            }
        }
    }

    void UpdateVisualColor()
    {
        if (portalRenderer != null && channelColors.ContainsKey(portalColor))
        {
            Material mat = new Material(portalRenderer.sharedMaterial);
            Color targetColor = channelColors[portalColor];

            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", targetColor);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", targetColor);
            if (mat.HasProperty("_EmissionColor")) mat.SetColor("_EmissionColor", targetColor * 2f);

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
        isCoolingDown = true;
        linkedTarget.SetCooldown(true);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        Vector3 originalScale = player.transform.localScale;
        float timer = 0f;

        // --- Phase 1: Enter (VFX & Shrink) ---
        SpawnEffect(transform.position);

        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            // "Inhale" effect: slightly scale up then shrink to zero
            float scaleCurve = Mathf.Sin(t * Mathf.PI) * 0.2f;
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t) + (Vector3.one * scaleCurve);
            player.transform.position = Vector3.Lerp(player.transform.position, transform.position, t);
            yield return null;
        }

        // --- Phase 2: Teleport ---
        player.transform.position = linkedTarget.transform.position;

        // --- Phase 3: Exit (VFX & Grow) ---
        linkedTarget.SpawnEffect(linkedTarget.transform.position);

        timer = 0f;
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            player.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }
        player.transform.localScale = originalScale;

        if (cc) cc.enabled = true;

        yield return new WaitForSeconds(0.5f);
        isCoolingDown = false;
        linkedTarget.SetCooldown(false);
    }

    void SpawnEffect(Vector3 pos)
    {
        if (teleportVFX != null)
        {
            GameObject vfx = Instantiate(teleportVFX, pos, Quaternion.identity);
            // Auto-color the particle system if it exists
            var ps = vfx.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = channelColors[portalColor];
            }
            Destroy(vfx, 2f);
        }
    }

    public void SetCooldown(bool state) => isCoolingDown = state;

    // 🔥 THE TD FEATURE: Visualizing the Box Collider color in Scene View 🔥
    void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();

        Color c = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;

        // Draw the connecting line
        if (linkedTarget != null)
        {
            Gizmos.color = c;
            Gizmos.DrawLine(transform.position, linkedTarget.transform.position);
        }

        // --- Draw the Box Collider Color ---
        Gizmos.matrix = transform.localToWorldMatrix;

        // 1. Solid-ish fill
        Gizmos.color = new Color(c.r, c.g, c.b, 0.3f);
        Gizmos.DrawCube(myCollider.center, myCollider.size);

        // 2. Bright wireframe outline
        Gizmos.color = c;
        Gizmos.DrawWireCube(myCollider.center, myCollider.size);
    }
}