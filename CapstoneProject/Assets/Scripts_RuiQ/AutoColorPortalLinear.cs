using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoColorPortalLinear : MonoBehaviour
{
    public enum PortalChannel
    {
        Red, Blue, Green, Yellow, Purple, Cyan
    }

    [Header("📢 Settings")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("🎨 Visuals")]
    public MeshRenderer portalRenderer;
    public GameObject teleportVFX;

    [Header("⏱️ Linear Force Settings")]
    [Tooltip("The time it takes to move to center and eject.")]
    public float actionDuration = 0.3f;
    [Tooltip("The linear distance the player is pushed forward.")]
    public float ejectionDistance = 3.5f;

    // --- Internal Variables ---
    private AutoColorPortalLinear linkedTarget;
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

    void OnValidate() { UpdateVisualColor(); }

    public void FindMyPartner()
    {
        var allPortals = Object.FindObjectsByType<AutoColorPortalLinear>(FindObjectsSortMode.None);
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
        Vector3 enterStartPos = player.transform.position;
        float timer = 0f;

        // --- Phase 1: Linear Inhale (向内的力) ---
        SpawnEffect(transform.position);
        while (timer < actionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / actionDuration;
            // Pure linear movement to center
            player.transform.position = Vector3.Lerp(enterStartPos, transform.position, t);
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }

        // --- Phase 2: Instant Swap ---
        player.transform.position = linkedTarget.transform.position;
        yield return new WaitForEndOfFrame();

        // --- Phase 3: Linear Eject (向外的力) ---
        linkedTarget.SpawnEffect(linkedTarget.transform.position);

        // Target is directly along the Forward axis (Blue arrow)
        Vector3 ejectEndPos = linkedTarget.transform.position + linkedTarget.transform.forward * ejectionDistance;

        timer = 0f;
        while (timer < actionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / actionDuration;
            // Pure linear movement to the ejection point
            player.transform.position = Vector3.Lerp(linkedTarget.transform.position, ejectEndPos, t);
            player.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }

        player.transform.localScale = originalScale;
        player.transform.position = ejectEndPos;

        if (cc) cc.enabled = true;

        yield return new WaitForSeconds(0.5f);
        isCoolingDown = false;
        linkedTarget.SetCooldown(false);
    }

    void SpawnEffect(Vector3 pos)
    {
        if (teleportVFX != null)
        {
            GameObject vfx = Instantiate(teleportVFX, pos, transform.rotation);
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

    void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();
        Color c = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;

        if (linkedTarget != null)
        {
            Gizmos.color = c;
            Gizmos.DrawLine(transform.position, linkedTarget.transform.position);
            // Visualization of the Ejection Force
            Gizmos.DrawRay(linkedTarget.transform.position, linkedTarget.transform.forward * ejectionDistance);
        }

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(c.r, c.g, c.b, 0.3f);
        Gizmos.DrawCube(myCollider.center, myCollider.size);
        Gizmos.color = c;
        Gizmos.DrawWireCube(myCollider.center, myCollider.size);
    }
}