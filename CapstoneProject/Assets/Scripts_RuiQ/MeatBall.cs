using UnityEngine;

public class MeatballImpact : MonoBehaviour
{
    [Header("💥 VFX Settings")]
    [Tooltip("拖入你的爆炸/溅射特效 Prefab")]
    public GameObject impactVFXPrefab;

    [Tooltip("特效播放多久后自动销毁 (秒)")]
    public float vfxLifetime = 2f;

    // 碰撞到任何物体时触发
    private void OnCollisionEnter(Collision collision)
    {
        // 1. 如果配置了特效，就在当前位置生成它
        if (impactVFXPrefab != null)
        {
            // 在肉丸的中心点生成特效，不旋转
            GameObject vfxInstance = Instantiate(impactVFXPrefab, transform.position, Quaternion.identity);

            // 重要：让生成的特效在几秒后自动销毁，防止Hierarchy堆满垃圾
            Destroy(vfxInstance, vfxLifetime);
        }

        // 2. 立即销毁肉丸自己
        // 这步能彻底解决你之前遇到的地面堆积问题
        Destroy(gameObject);
    }
}