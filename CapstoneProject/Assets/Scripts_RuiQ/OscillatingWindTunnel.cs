using UnityEngine;
using System.Collections;
using lilGuysNamespace;

public class OscillatingWindTunnel : MonoBehaviour
{
    [Header("📍 范围检测")]
    public Vector3 detectionSize = new Vector3(10f, 5f, 5f); // 横向长条
    public Vector3 centerOffset = Vector3.zero;

    [Header("🌊 正弦波设置")]
    [Tooltip("勾选它！(已默认勾选)")]
    public bool useSineWaveMode = true;

    [Tooltip("完整呼吸一次需要多少秒？\n(左->右->左 一个来回)\n建议填 6 到 10。")]
    public float waveCycleTime = 8f;

    [Header("🚀 风力参数")]
    [Tooltip("风的推力加速度 (推背感)\n正弦波因为有渐变，建议给大一点，比如 30-40。")]
    public float windAcceleration = 35f;

    [Tooltip("最大风速 (限制)")]
    public float maxWindSpeed = 15f;

    [Header("✨ 特效 (必填)")]
    [Tooltip("往左吹时播放的粒子")]
    public ParticleSystem leftParticles;
    [Tooltip("往右吹时播放的粒子")]
    public ParticleSystem rightParticles;

    // 内部变量
    private float currentWindStrength = 0f; // -1 (左) 到 1 (右)
    private Transform playerTransform;
    private CharacterController playerCC;
    private EntityManager playerManager;
    private bool isPlayerInside = false;

    void Start()
    {
        FindActivePlayer();
        if (GetComponent<Rigidbody>()) Destroy(GetComponent<Rigidbody>());

        // 根据选择启动不同模式
        if (useSineWaveMode)
            StartCoroutine(SineWaveCycle());
        else
            StartCoroutine(HardSwitchCycle());
    }

    void Update()
    {
        // 🛡️ 防崩溃检查
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

        if (!isPlayerInside) return;

        // ================= 🌪️ 施加风力 =================

        // 死区处理：如果风力太微弱 (比如 < 0.1)，就当做无风，让玩家休息
        if (Mathf.Abs(currentWindStrength) < 0.1f) return;

        Vector3 currentVel = playerManager.GetMovementVelocity();

        // 确定方向：transform.right 是红色轴
        // currentWindStrength 为正 -> 往右；为负 -> 往左
        Vector3 targetDirection = transform.right * Mathf.Sign(currentWindStrength);

        // 计算实际推力：强度 * 加速度
        // 正弦波在顶峰时强度是 1，在低谷是 0，这会自动产生“渐强渐弱”的手感
        float finalAccel = windAcceleration * Mathf.Abs(currentWindStrength);

        Vector3 windForce = targetDirection * finalAccel * Time.deltaTime;
        Vector3 newVel = currentVel + windForce;

        // 限速 (只限侧向)
        float speedInWindDir = Vector3.Dot(newVel, targetDirection);
        if (speedInWindDir > maxWindSpeed)
        {
            Vector3 correction = targetDirection * (speedInWindDir - maxWindSpeed);
            newVel -= correction;
        }

        playerManager.SetMovementVelocity(newVel);
    }

    // 🌊 正弦波循环 (丝滑过渡)
    IEnumerator SineWaveCycle()
    {
        float timer = 0f;
        while (true)
        {
            timer += Time.deltaTime;

            // 核心公式：生成 -1 到 1 的平滑波形
            // 用 Mathf.Sin 产生自然波动
            float wave = Mathf.Sin((timer / waveCycleTime) * Mathf.PI * 2);

            currentWindStrength = wave;

            // 粒子特效控制 (平滑开关)
            // 当向左吹的强度 > 0.2 时，开启左粒子
            if (wave < -0.2f)
            {
                if (leftParticles && !leftParticles.isPlaying) leftParticles.Play();
                if (rightParticles) rightParticles.Stop();
            }
            // 当向右吹的强度 > 0.2 时，开启右粒子
            else if (wave > 0.2f)
            {
                if (leftParticles) leftParticles.Stop();
                if (rightParticles && !rightParticles.isPlaying) rightParticles.Play();
            }
            // 中间微弱过渡期 (静风期)
            else
            {
                if (leftParticles) leftParticles.Stop();
                if (rightParticles) rightParticles.Stop();
            }

            yield return null; // 等待下一帧
        }
    }

    // 备用：硬切换模式
    IEnumerator HardSwitchCycle()
    {
        // (如果以后想切回硬模式，把 Inspector 里的勾去掉即可)
        float blowDuration = 3f;
        float pauseDuration = 2f;
        while (true)
        {
            currentWindStrength = -1f; // 左
            if (leftParticles) leftParticles.Play(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(blowDuration);

            currentWindStrength = 0f; // 停
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(pauseDuration);

            currentWindStrength = 1f; // 右
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Play();
            yield return new WaitForSeconds(blowDuration);

            currentWindStrength = 0f; // 停
            if (leftParticles) leftParticles.Stop(); if (rightParticles) rightParticles.Stop();
            yield return new WaitForSeconds(pauseDuration);
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
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawCube(centerOffset, detectionSize);
        Gizmos.DrawWireCube(centerOffset, detectionSize);

        // 动态箭头：根据当前风力和方向变化
        Vector3 right = Vector3.right * (detectionSize.x * 0.4f);
        if (Mathf.Abs(currentWindStrength) > 0.1f)
        {
            Gizmos.color = currentWindStrength > 0 ? Color.red : Color.blue;
            Gizmos.DrawRay(centerOffset, right * currentWindStrength);
            Gizmos.DrawSphere(centerOffset + right * currentWindStrength, 0.4f);
        }
    }

    void OnGUI()
    {
        if (!isPlayerInside || !playerCC.gameObject.activeInHierarchy) return;
        GUIStyle style = new GUIStyle();
        style.fontSize = 24;

        string dirText = "⏸️ 静风过渡区";
        if (currentWindStrength > 0.2f) { dirText = "➡️ 强力右风"; style.normal.textColor = Color.red; }
        else if (currentWindStrength < -0.2f) { dirText = "⬅️ 强力左风"; style.normal.textColor = Color.blue; }
        else style.normal.textColor = Color.white;

        GUI.Label(new Rect(20, 20, 500, 100), $"{dirText} (强度: {currentWindStrength:F2})", style);
    }
}