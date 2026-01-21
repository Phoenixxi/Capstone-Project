using UnityEngine;
using System.Collections; // 必须引用这个

[RequireComponent(typeof(Rigidbody))]
public class PendulumTrap : MonoBehaviour
{
    [Header("⚙️ 摆动设置")]
    public Vector3 swingAxis = new Vector3(0, 0, 1); // (0,0,1)左右摆, (1,0,0)前后摆
    public float speed = 1.5f;
    public float angleLimit = 75f;
    public float timeOffset = 0f;

    [Header("💥 击飞设置 (重点)")]
    [Tooltip("推力大小，建议设大一点，比如 20 或 30")]
    public float pushForce = 25f;

    [Tooltip("击飞持续时间(秒)，时间越长飞得越远")]
    public float pushDuration = 0.25f;

    private Rigidbody rb;
    private Quaternion startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        startRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        float currentAngle = Mathf.Sin((Time.time + timeOffset) * speed) * angleLimit;
        Quaternion rotationOffset = Quaternion.Euler(swingAxis * currentAngle);
        rb.MoveRotation(startRotation * rotationOffset);
    }

    // ⚡ 碰撞检测
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 获取玩家的控制器
            CharacterController playerCC = collision.gameObject.GetComponent<CharacterController>();

            if (playerCC != null)
            {
                // 1. 计算击飞方向 (从锤子中心 -> 玩家)
                Vector3 dir = (collision.transform.position - transform.position).normalized;

                // 2. 稍微给一点向上的力，防止被地面摩擦力抵消
                dir += Vector3.up * 0.2f;
                dir.Normalize();

                // 3. 启动协程：在锤子上运行代码去推玩家
                // 这样就不需要玩家身上有脚本了
                StartCoroutine(PushPlayerRoutine(playerCC, dir));
            }
        }
    }

    // ⚡ 推人的协程
    IEnumerator PushPlayerRoutine(CharacterController cc, Vector3 direction)
    {
        float timer = 0f;

        // 在 pushDuration 时间内，每一帧都推玩家一下
        while (timer < pushDuration)
        {
            // 如果玩家中途死了或没了，停止推
            if (cc == null) yield break;

            // 模拟受力衰减 (一开始劲大，后面劲小)
            float currentForce = Mathf.Lerp(pushForce, 0, timer / pushDuration);

            // 强制移动玩家
            cc.Move(direction * currentForce * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null; // 等待下一帧
        }
    }
}