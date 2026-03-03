using UnityEngine;

public class Meatball : MonoBehaviour
{
    [Header("🚀 Physics")]
    public float maxSpeed = 25f; // 由 Dropper 脚本动态同步这个值

    [SerializeField] private float damage = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 【方案 A：后备方案】
        // 即使没撞到东西（比如掉出地图），10秒后也必须强制销毁，堆积问题
        Destroy(gameObject, 10f);
    }

    void FixedUpdate()
    {
        // 【限速逻辑】
        // 在物理帧检查速度，如果超过最大速度就截断
        if (rb != null && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    // 【方案 B：碰撞即毁】
    private void OnCollisionEnter(Collision collision)
    {
        // 只要碰到任何物体（地面、墙壁、玩家、其他球），就立刻消失
        // 这能彻底解决 image_03a859 里的 Tag 报错问题，因为它不需要检查标签

        // 如果你有特效（VFX）预制体，可以在这里 Instantiate
        // Instantiate(impactVFX, transform.position, Quaternion.identity);

        if(collision.gameObject.CompareTag("Player"))
        {
            EntityManager entityManager = collision.gameObject.GetComponentInChildren<EntityManager>();
            entityManager.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}