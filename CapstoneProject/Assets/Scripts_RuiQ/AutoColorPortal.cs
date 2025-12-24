using System.Collections;
using System.Linq;
using UnityEngine;

public class AutoColorPortal : MonoBehaviour
{
    public enum PortalChannel
    {
        Red, Blue, Green, Yellow, Purple, Cyan
    }

    [Header("📢 设置")]
    [Tooltip("选择颜色，脚本会自动连接场景里另一个同色的门")]
    public PortalChannel portalColor = PortalChannel.Red;

    [Header("🎨 视觉")]
    [Tooltip("把圆盘模型拖到这里，自动改颜色")]
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
    private AutoColorPortal linkedTarget;
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
        var allPortals = FindObjectsByType<AutoColorPortal>(FindObjectsSortMode.None);
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
            StartCoroutine(TeleportProcess(other.gameObject));
        }
    }

    private IEnumerator TeleportProcess(GameObject player)
    {
        // 1. 禁用控制
        CharacterController cc = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (cc) cc.enabled = false;
        if (rb) rb.isKinematic = true;

        Vector3 startScale = player.transform.localScale;
        Vector3 startPos = player.transform.position;
        float timer = 0f;

        // --- 第一阶段：吸入变小 ---
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;
            player.transform.position = Vector3.Lerp(startPos, transform.position, t);
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
        player.transform.localScale = Vector3.zero;

        // --- 第二阶段：传送 ---
        linkedTarget.StartCooldown();
        player.transform.position = linkedTarget.transform.position;

        // --- 第三阶段：🔥 获取目标门的 X 轴方向 🔥 ---
        // 直接读取出口门 (linkedTarget) 的 transform.right (红轴方向)
        // 如果你的门旋转了，这个方向就会变
        Vector3 portalRight = linkedTarget.transform.right;

        // 强制归一化成纯粹的 左(-1) 或 右(1)，防止门歪了导致斜着飞
        Vector3 throwDir = (portalRight.x >= 0) ? Vector3.right : Vector3.left;

        // --- 第四阶段：同时变大 + 弹射 ---
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

        // --- 第五阶段：恢复控制 ---
        if (cc) cc.enabled = true;
        if (rb) rb.isKinematic = false;
    }

    private IEnumerator ThrowPlayerSideScroll(GameObject player, AutoColorPortal originPortal, Vector3 direction)
    {
        float elapsed = 0f;
        Vector3 p0 = originPortal.transform.position;

        // 计算落点
        Vector3 p2 = p0 + (direction * throwDistance);

        // 控制点(最高点)
        Vector3 p1 = p0 + (direction * (throwDistance / 2)) + (Vector3.up * throwHeight);

        // 锁死 Z 轴
        float fixedZ = player.transform.position.z;

        while (elapsed < flightDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flightDuration;

            // 贝塞尔曲线
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
            Gizmos.color = channelColors.ContainsKey(portalColor) ? channelColors[portalColor] : Color.white;
            Gizmos.DrawLine(transform.position, linkedTarget.transform.position);

            // 画出当前门的弹出方向，方便你调试
            Vector3 dir = transform.right.x >= 0 ? Vector3.right : Vector3.left;
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, dir * 2f);
        }
    }
}