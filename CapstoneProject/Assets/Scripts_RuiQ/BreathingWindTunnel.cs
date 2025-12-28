using UnityEngine;
using System.Collections;
using lilGuysNamespace;

public class BreathingWindTunnel : MonoBehaviour
{
    [Header("📍 范围检测")]
    public Vector3 detectionSize = new Vector3(3f, 5f, 3f);
    public Vector3 centerOffset = Vector3.zero;

    [Header("📏 激活条件 (脚下踩空才飞)")]
    [Tooltip("脚下多少米悬空才触发？建议 0.8")]
    public float activationHeight = 0.8f;
    public LayerMask groundLayer = ~0; // 默认检测所有层

    [Header("🌬️ 呼吸节奏")]
    [Tooltip("向上喷射持续多久？(秒)")]
    public float blowDuration = 2.5f;
    [Tooltip("缓降休息持续多久？(秒)")]
    public float sinkDuration = 3f;

    [Header("🚀 飞行参数")]
    [Tooltip("向上喷射时的加速度：\n填 40，保证起飞有力。")]
    public float blowAcceleration = 40f;

    [Tooltip("向上喷射的最大速度：\n填 50，防止飞太快崩游戏。")]
    public float maxBlowSpeed = 50f;

    [Tooltip("缓降时的恒定速度 (必须是负数)：\n填 -3。\n这就是你要的“抵消重力”。\n在休息阶段，你的速度会被锁死在 -3，匀速下降，绝不会越来越快。")]
    public float sinkSpeed = -3f;

    [Header("✨ 特效")]
    public ParticleSystem upParticles;
    public ParticleSystem weakParticles; // 建议给缓降也加个弱弱的特效

    // 内部状态
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
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());

        // 启动呼吸循环
        StartCoroutine(BreathCycle());
    }

    void Update()
    {
        // 🛡️ 防崩溃：如果玩家被禁用了，直接停止
        if (playerCC == null || playerManager == null || !playerCC.gameObject.activeInHierarchy)
        {
            FindActivePlayer();
            if (playerCC == null) return;
        }
        if (!playerCC.enabled) return;

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

        // 2. 悬空检测 (智能锁)
        // 只有脚下 activationHeight 范围内没有东西，才算“踩空”
        bool hitGround = Physics.Raycast(playerTransform.position, Vector3.down, activationHeight, groundLayer);
        isAirborneEnough = !hitGround;

        // 如果脚下有地，直接 Return，让玩家正常走路，风洞完全不干涉！
        if (!isAirborneEnough) return;

        // ================= 🌬️ 核心物理逻辑 =================

        Vector3 currentVel = playerManager.GetMovementVelocity();
        float currentY = currentVel.y;

        if (currentState == WindState.Blowing)
        {
            // --- 🔥 呼气阶段 (向上喷射) ---

            // 如果是从下落状态刚转过来，先给一个强力的反向修正，消除坠落惯性
            if (currentY < 0)
            {
                currentY = Mathf.MoveTowards(currentY, maxBlowSpeed, blowAcceleration * 2f * Time.deltaTime);
            }
            else
            {
                // 正常加速
                currentY = Mathf.MoveTowards(currentY, maxBlowSpeed, blowAcceleration * Time.deltaTime);
            }
        }
        else
        {
            // --- 🍃 吸气阶段 (抵消重力的缓降) ---

            // 这里的逻辑是：
            // 如果你现在的速度比 sinkSpeed (-3) 还要快 (比如你还在往上冲)，那就让重力自然把你拉下来。
            // 但是！一旦你的速度掉到了 -3，我就开启“反重力引擎”，强行顶住你。

            if (currentY > sinkSpeed)
            {
                // 此时你还在上升或者慢速下落，我们让重力自然发挥，或者稍微给点阻力让过渡平滑
                // 这里用 MoveTowards 让速度慢慢降到 -3
                currentY = Mathf.MoveTowards(currentY, sinkSpeed, 10f * Time.deltaTime);
            }
            else
            {
                // ⚠️ 关键点：防止越来越快 ⚠️
                // 此时重力想把你拉到 -10, -20...
                // 我们直接锁死在 -3。这就在物理上等同于“风力 = 重力”。
                currentY = sinkSpeed;
            }
        }

        // 应用速度
        playerManager.SetMovementVelocity(new Vector3(currentVel.x, currentY, currentVel.z));
    }

    // 🔄 呼吸循环
    IEnumerator BreathCycle()
    {
        while (true)
        {
            // 1. 喷射模式
            currentState = WindState.Blowing;
            if (upParticles) upParticles.Play();
            if (weakParticles) weakParticles.Stop();
            yield return new WaitForSeconds(blowDuration);

            // 2. 缓降模式
            currentState = WindState.Sinking;
            if (upParticles) upParticles.Stop();
            if (weakParticles) weakParticles.Play(); // 此时可以播放一个微弱的气流特效
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

    void OnGUI()
    {
        if (playerManager == null || !playerCC.gameObject.activeInHierarchy) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 24;

        if (isPlayerInside)
            style.normal.textColor = isAirborneEnough ? Color.green : Color.yellow;
        else
            style.normal.textColor = Color.red;

        string stateText = "⏸️ 地面待机";
        if (isPlayerInside && isAirborneEnough)
        {
            stateText = currentState == WindState.Blowing ? "💨 向上喷射" : "🍃 恒速缓降";
        }

        float velY = playerManager.GetMovementVelocity().y;
        GUI.Label(new Rect(20, 20, 900, 100), $"{stateText} | 速度: {velY:F1}", style);
    }
}