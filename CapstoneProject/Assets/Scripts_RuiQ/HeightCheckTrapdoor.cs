using UnityEngine;
using lilGuysNamespace; // 引用你的命名空间以获取玩家引用

public class HeightCheckTrapdoor : MonoBehaviour
{
    [Header("📏 判定高度")]
    [Tooltip("当玩家的Y轴坐标超过 [平台Y轴 + 这个数值] 时，平台会出现。\n建议填 0.5 或 1.0 (确保玩家完全飞过去了再关门)。")]
    public float heightOffset = 0.5f;

    [Header("🚪 门的设置")]
    [Tooltip("一开始是否隐藏？(必须勾选，否则还没飞上来路就被堵住了)")]
    public bool startHidden = true;

    [Tooltip("关门时是否播放特效/音效")]
    public ParticleSystem appearEffect;
    public AudioSource audioSource;

    private Transform playerTransform;
    private Collider myCollider;
    private Renderer myRenderer;
    private bool isClosed = false;

    void Start()
    {
        // 自动找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) playerTransform = playerObj.transform;

        // 获取自身的组件
        myCollider = GetComponent<Collider>();
        myRenderer = GetComponent<Renderer>();

        // 初始化状态
        if (startHidden)
        {
            SetDoorState(false); // 先隐藏
        }
    }

    void Update()
    {
        // 如果门已经关上了，或者找不到玩家，就不用检测了
        if (isClosed || playerTransform == null) return;

        // 🔥🔥🔥 核心逻辑：高度比对 🔥🔥🔥
        // 平台的 Y 轴位置
        float doorHeight = transform.position.y;

        // 玩家的 Y 轴位置
        float playerHeight = playerTransform.position.y;

        // 如果 [玩家高度] > [门高度 + 偏移量]
        // 说明玩家已经飞到板子上面去了
        if (playerHeight > (doorHeight + heightOffset))
        {
            CloseTheDoor();
        }
    }

    void CloseTheDoor()
    {
        isClosed = true;
        SetDoorState(true); // 显示门，开启碰撞

        // 播放特效/音效
        if (appearEffect != null) appearEffect.Play();
        if (audioSource != null) audioSource.Play();

        Debug.Log("🚪 检测到玩家已通过，活板门关闭！");
    }

    // 统一控制显示/隐藏
    void SetDoorState(bool active)
    {
        // 控制碰撞体 (防止隐形时撞头)
        if (myCollider != null) myCollider.enabled = active;

        // 控制画面 (防止还没上去就看见板子)
        if (myRenderer != null) myRenderer.enabled = active;

        // 如果有子物体也一起控制
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(active);
        }
    }

    void OnDrawGizmos()
    {
        // 画一条线，告诉你玩家超过哪条线门才会关
        Gizmos.color = Color.yellow;
        Vector3 linePos = transform.position;
        linePos.y += heightOffset;

        Gizmos.DrawLine(linePos + Vector3.left * 2, linePos + Vector3.right * 2);
        Gizmos.DrawIcon(linePos, "DoorThreshold");
    }
}