using UnityEngine;
using lilGuysNamespace;

public class BreathingWindTunnel : MonoBehaviour
{
    [Header("📍 范围检测")]
    public Vector3 detectionSize = new Vector3(3f, 5f, 3f);
    public Vector3 centerOffset = Vector3.zero;

    [Header("📏 激活条件 (脚下踩空才飞)")]
    [Tooltip("脚下多少米悬空才触发？建议 0.5 - 1.0")]
    public float activationHeight = 0.8f;
    public LayerMask groundLayer = 1;

    [Header("🚀 无限喷射参数")]
    [Tooltip("起步初速度：\n一旦触发，直接给你这个速度，绝不含糊。\n保证你瞬间从“下落”变成“起飞”。建议 15。")]
    public float initialKickSpeed = 15f;

    [Tooltip("加速度：\n每秒增加的速度。\n因为没有最大速度限制，这个值决定了你变快的节奏。\n建议 30-50。")]
    public float acceleration = 40f;

    [Header("✨ 特效")]
    public ParticleSystem upParticles;

    private Transform playerTransform;
    private CharacterController playerCC;
    private EntityManager playerManager;
    private bool isPlayerInside = false;
    private bool isAirborneEnough = false;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
            playerCC = playerObj.GetComponent<CharacterController>();
            playerManager = playerObj.GetComponent<EntityManager>();

            if (playerCC == null) playerCC = playerObj.GetComponentInChildren<CharacterController>();
            if (playerManager == null) playerManager = playerObj.GetComponentInChildren<EntityManager>();
        }

        if (upParticles != null) upParticles.Play();
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());
        if (groundLayer == 0 || groundLayer == 1) groundLayer = ~0;
    }

    void Update()
    {
        if (playerTransform == null || playerManager == null) return;

        // 1. 范围检测
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

        // 2. 悬空检测
        // 只有脚下 activationHeight 范围内没有地，才算“踩空”
        bool hitGround = Physics.Raycast(playerTransform.position, Vector3.down, activationHeight, groundLayer);
        isAirborneEnough = !hitGround;

        // 如果脚下有地，直接 Return，让玩家正常走路/跳跃
        if (!isAirborneEnough) return;

        // ================= 🚀 无限加速逻辑 =================

        Vector3 currentVel = playerManager.GetMovementVelocity();
        float currentY = currentVel.y;

        // 3. 第一步：消除下坠，保证起步
        // 如果当前是在往下掉，或者向上速度还不如初速度快
        // 直接暴力覆盖为 initialKickSpeed
        if (currentY < initialKickSpeed)
        {
            // 用 MoveTowards 快速拉升 (几乎瞬间)，防止瞬移感太强，但必须极快
            currentY = Mathf.MoveTowards(currentY, initialKickSpeed, acceleration * 10f * Time.deltaTime);
        }
        else
        {
            // 4. 第二步：无限叠加
            // 只要已经在向上了，就每一帧都加 acceleration
            // 没有 maxWindSpeed 限制！没有封顶！
            // 只要你在风洞里待得越久，你就会飞得越快，直到光速。
            currentY += acceleration * Time.deltaTime;
        }

        // 5. 应用
        playerManager.SetMovementVelocity(new Vector3(currentVel.x, currentY, currentVel.z));
    }

    void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        Gizmos.color = isPlayerInside ? Color.green : new Color(1, 0, 0, 0.3f);
        Gizmos.DrawCube(centerOffset, detectionSize);
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(centerOffset, detectionSize);
    }

    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;
        if (isPlayerInside)
            style.normal.textColor = isAirborneEnough ? Color.green : Color.yellow;
        else
            style.normal.textColor = Color.red;

        string statusText = "不在风洞";
        if (isPlayerInside) statusText = isAirborneEnough ? "🚀 无限加速中" : "🚶 地面待机";

        float velY = playerManager != null ? playerManager.GetMovementVelocity().y : 0;

        // 显示当前速度，你会看到这个数字无限上涨
        GUI.Label(new Rect(20, 20, 900, 100),
            $"{statusText} | 当前速度: {velY:F1} (无上限)", style);
    }
}