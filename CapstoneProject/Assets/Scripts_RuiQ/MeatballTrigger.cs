using UnityEngine;

public class MeatballTrigger : MonoBehaviour
{
    [Header("🔗 Target Settings")]
    [Tooltip("拖入场景中那个挂有 MeatballDropper 脚本的物体")]
    public MeatballDropper targetDropper;

    [Header("🛠️ Configuration")]
    [Tooltip("离场时是否停止掉落？")]
    public bool stopOnExit = true;

    // 当物体进入触发区
    private void OnTriggerEnter(Collider other)
    {
        // 关键：检查进入的是不是玩家
        if (other.CompareTag("Player"))
        {
            if (targetDropper != null)
            {
                targetDropper.SetSpawning(true);
            }
        }
    }

    // 当物体离开触发区
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && stopOnExit)
        {
            if (targetDropper != null)
            {
                targetDropper.SetSpawning(false);
            }
        }
    }
}