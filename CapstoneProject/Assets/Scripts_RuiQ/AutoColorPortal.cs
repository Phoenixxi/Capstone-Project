using System.Collections;
using System.Linq;
using UnityEngine;

// ⚠️ 注意：类名现在叫 Portal，必须和你的文件名 Portal.cs 一样！
public class Portal : MonoBehaviour
{
    // 定义颜色通道
    public enum PortalChannel
    {
        Red, Blue, Green, Yellow, Purple, Cyan
    }

    [Header("📢 设置")]
    [Tooltip("选择颜色，自动连接场景里另一个同色的门")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("🎨 视觉")]
    public MeshRenderer portalRenderer;

    [Header("⏱️ 动画设置")]
    [Tooltip("变大变小的时间 (秒)")]
    public float animationDuration = 0.5f;

    [Header("🚀 横版抛出设置")]
    [Tooltip("水平抛出的距离 (X轴)")]
    public float throwDistance = 5f;
    [Tooltip("抛出的高度 (Y轴)")]
    public float throwHeight = 3f;
    [Tooltip("飞行时间 (秒)")]
    public float flightDuration = 0.8f;

    // --- 内部变量 ---
    private Portal linkedTarget;
    private bool isCoolingDown = false;
    private Collider myCollider;

    // 颜色字典
    private readonly System.Collections.Generic.Dictionary<PortalChannel, Color> channelColors = new System.Collections.Generic.Dictionary<PortalChannel, Color>()
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
        // Unity 6 API: FindObjectsByType
        var allPortals = FindObjectsByType<Portal>(FindObjectsSortMode.None);
        foreach (var portal in allPortals)
        {
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
            Rigidbody rb = other.GetComponent<Rigidbody>();
            float speedX = 0f;

            if (rb != null)
            {
                // Unity 6: linearVelocity
                speedX = rb.linearVelocity.x;
            }

            // 如果速度太小，默认向右
            if (Mathf.Abs(speedX) < 0.1f) speedX = 1f;

            StartCoroutine(TeleportProcess(other.gameObject, speedX));
        }
    }

    private IEnumerator TeleportProcess(GameObject player, float speedX)
    {
        // 1. 禁用控制
        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (cc) cc.enabled = false;
        if (rb) rb.isKinematic = true;

        Vector3 startScale = player.transform.localScale;
        Vector3 startPos = player.transform.position;
        float timer = 0f;

        // --- Phase 1: 吸入变小 ---
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            player.transform.position = Vector3.Lerp(startPos, transform.position, t);
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        player.transform.localScale = Vector3.zero;

        // --- Phase 2: 传送 ---
        linkedTarget.StartCooldown();
        player.transform.position = linkedTarget.transform.position;

        // --- Phase 3: 计算横版方向 (向左 or 向右) ---
        Vector3 throwDir = (speedX >= 0) ? Vector3.right : Vector3.left;

        // --- Phase 4: 边飞边变大 ---
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

        // --- Phase 5: 恢复控制 ---
        if (cc) cc.enabled = true;
        if (rb)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator ThrowPlayerSideScroll(GameObject player, Portal originPortal, Vector3 direction)
    {
        float elapsed = 0f;
        Vector3 p0 = originPortal.transform.position;

        // 终点
        Vector3 p2 = p0 + (direction * throwDistance);

        // 最高点
        Vector3 p1 = p0 + (direction * (throwDistance / 2)) + (Vector3.up * throwHeight);

        // 记录 Z 轴防止飞歪
        float fixedZ = player.transform.position.z;

        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flightDuration;

            // 贝塞尔曲线
            Vector3 position = Mathf.Pow(1 - t, 2) * p0 + 2 * (1 - t) * t * p1 + Mathf.Pow(t, 2) * p2;

            // 锁定 Z 轴
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
            Gizmos.color = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;
            Gizmos.DrawLine(transform.position, linkedTarget.transform.position);
        }
    }
}