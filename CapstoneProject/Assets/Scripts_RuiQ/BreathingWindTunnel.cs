using UnityEngine;
using System.Collections;
using lilGuysNamespace;

public class BreathingWindTunnel : MonoBehaviour
{
    [Header("🌬️ 风力设置")]
    [Tooltip("向上推的速度")]
    public float blowSpeed = 12f;
    [Tooltip("向下吸的速度")]
    public float suckSpeed = 12f;

    [Header("⏱️ 呼吸循环")]
    public float blowTime = 3f;
    public float restTime = 2f;
    public float suckTime = 3f;

    [Header("✨ 特效")]
    public ParticleSystem upParticles;
    public ParticleSystem downParticles;

    // 内部状态
    private enum WindState { Resting, BlowingUp, SuckingDown }
    private WindState currentState = WindState.BlowingUp;

    // 🔥 关键修改：用来记录当前在风洞里的玩家
    private CharacterController activePlayerCC;

    void Start()
    {
        // 自动补全组件
        var box = GetComponent<BoxCollider>();
        if (box == null) box = gameObject.AddComponent<BoxCollider>();
        if (!box.isTrigger) box.isTrigger = true;

        var rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        StartCoroutine(BreathingCycle());
    }

    // 🔥 关键修改：把推人的逻辑搬到了 Update 里
    // Update 是跟渲染帧率同步的，每秒60帧或者144帧，非常丝滑
    void Update()
    {
        // 只有当玩家在风洞里，且风洞不在休息时，才推
        if (activePlayerCC != null && currentState != WindState.Resting)
        {
            Vector3 moveDir = Vector3.zero;

            if (currentState == WindState.BlowingUp)
            {
                moveDir = Vector3.up * blowSpeed;
            }
            else if (currentState == WindState.SuckingDown)
            {
                moveDir = Vector3.down * suckSpeed;
            }

            // 使用 Time.deltaTime 确保平滑
            activePlayerCC.Move(moveDir * Time.deltaTime);
        }
    }

    IEnumerator BreathingCycle()
    {
        while (true)
        {
            // 1. 向上吹
            currentState = WindState.BlowingUp;
            if (upParticles) upParticles.Play();
            yield return new WaitForSeconds(blowTime);
            if (upParticles) upParticles.Stop();

            // 2. 休息
            currentState = WindState.Resting;
            yield return new WaitForSeconds(restTime);

            // 3. 向下吸
            currentState = WindState.SuckingDown;
            if (downParticles) downParticles.Play();
            yield return new WaitForSeconds(suckTime);
            if (downParticles) downParticles.Stop();

            // 4. 休息
            currentState = WindState.Resting;
            yield return new WaitForSeconds(restTime);
        }
    }

    // 进入风洞：登记玩家
    void OnTriggerEnter(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        // 兼容性查找
        if (cc == null) cc = other.GetComponentInParent<CharacterController>();
        if (cc == null) cc = other.GetComponentInChildren<CharacterController>();

        if (cc != null)
        {
            activePlayerCC = cc;
        }
    }

    // 离开风洞：注销玩家
    void OnTriggerExit(Collider other)
    {
        CharacterController cc = other.GetComponent<CharacterController>();
        if (cc == null) cc = other.GetComponentInParent<CharacterController>();
        if (cc == null) cc = other.GetComponentInChildren<CharacterController>();

        // 只有当离开的人是当前记录的人时，才清空
        if (cc != null && cc == activePlayerCC)
        {
            activePlayerCC = null;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        if (GetComponent<BoxCollider>())
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(GetComponent<BoxCollider>().center, GetComponent<BoxCollider>().size);
        }
    }
}