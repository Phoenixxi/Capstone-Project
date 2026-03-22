using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class AutoColorVFXPortal : MonoBehaviour
{
    public enum PortalChannel { Red, Blue, Green, Yellow, Purple, Cyan }

    [Header("📢 Portal Link")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("✨ VFX Settings")]
    public GameObject portalVFXPrefab;
    [Tooltip("勾选：碰到门立刻放特效；不勾：开始缩小时放特效")]
    public bool vfxOnTouch = true;

    [Header("⏱️ Speed Control (Game Feel)")]
    [Range(0f, 3f)]
    public float waitBeforeSuck = 0.3f; // 前摇
    [Range(0.1f, 5f)]
    public float movementDuration = 1.2f; // 你要的：被吸入过程的速度
    [Range(0f, 3f)]
    public float scaleDuration = 0.8f; // 缩小的速度

    [Header("📈 Animation Curve")]
    [Tooltip("控制吸入的力度感：起始慢，快到中心时变快（Ease In）会让吸入感更真实")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    // --- Internal ---
    private AutoColorVFXPortal linkedTarget;
    private bool isCoolingDown = false;
    private BoxCollider myCollider;

    private readonly Dictionary<PortalChannel, Color> channelColors = new Dictionary<PortalChannel, Color>()
    {
        { PortalChannel.Red, Color.red }, { PortalChannel.Blue, Color.blue }, { PortalChannel.Green, Color.green },
        { PortalChannel.Yellow, Color.yellow }, { PortalChannel.Purple, new Color(0.6f, 0f, 1f) }, { PortalChannel.Cyan, Color.cyan }
    };

    void Awake() { myCollider = GetComponent<BoxCollider>(); FindMyPartner(); }
    void OnValidate() { if (myCollider == null) myCollider = GetComponent<BoxCollider>(); }

    public void FindMyPartner()
    {
        var allPortals = Object.FindObjectsByType<AutoColorVFXPortal>(FindObjectsSortMode.None);
        foreach (var portal in allPortals)
        {
            if (portal != this && portal.portalColor == this.portalColor)
            {
                linkedTarget = portal;
                return;
            }
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

        // --- 🚀 修正 1：如果设置了立即触发，碰到就炸特效 ---
        if (vfxOnTouch) SpawnVFX(transform.position);

        // 阶段 A：前摇等待
        yield return new WaitForSeconds(waitBeforeSuck);

        // --- 🚀 修正 2：如果没设置立即触发，现在开始吸入时炸特效 ---
        if (!vfxOnTouch) SpawnVFX(transform.position);

        Vector3 originalScale = player.transform.localScale;
        Vector3 startPos = player.transform.position;
        float timer = 0f;

        // 阶段 B：吸入移动过程 (Movement)
        while (timer < movementDuration)
        {
            timer += Time.deltaTime;
            float t = movementCurve.Evaluate(timer / movementDuration);
            // 慢慢拉到中心
            player.transform.position = Vector3.Lerp(startPos, transform.position, t);
            yield return null;
        }
        player.transform.position = transform.position;

        // 阶段 C：缩小过程 (Shrink)
        timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float t = timer / scaleDuration;
            player.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            yield return null;
        }
        player.transform.localScale = Vector3.zero;

        // 阶段 D：传送与出口特效
        yield return new WaitForSeconds(0.1f);
        player.transform.position = linkedTarget.transform.position;

        // 🚀 出口特效（在目标门口触发）
        linkedTarget.SpawnVFX(linkedTarget.transform.position);

        // 阶段 E：变大恢复
        timer = 0f;
        while (timer < scaleDuration)
        {
            timer += Time.deltaTime;
            float t = timer / scaleDuration;
            player.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            yield return null;
        }
        player.transform.localScale = originalScale;

        if (cc) cc.enabled = true;
        yield return new WaitForSeconds(1.0f);
        isCoolingDown = false;
        linkedTarget.SetCooldown(false);
    }

    public void SpawnVFX(Vector3 pos)
    {
        if (portalVFXPrefab != null)
        {
            GameObject vfx = Instantiate(portalVFXPrefab, pos, Quaternion.identity);
            var ps = vfx.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = channelColors[portalColor];
            }
            Destroy(vfx, 3f);
        }
    }

    public void SetCooldown(bool state) => isCoolingDown = state;

    void OnDrawGizmos()
    {
        if (myCollider == null) myCollider = GetComponent<BoxCollider>();
        Color c = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(c.r, c.g, c.b, 0.3f);
        Gizmos.DrawCube(myCollider.center, myCollider.size);
        Gizmos.color = c;
        Gizmos.DrawWireCube(myCollider.center, myCollider.size);
    }
}