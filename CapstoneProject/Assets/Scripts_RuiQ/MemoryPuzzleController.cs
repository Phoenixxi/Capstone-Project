using UnityEngine;
using System.Collections.Generic;

public class TumorManager : MonoBehaviour
{
    [Header("🚪 目标门")]
    public GameObject doorToOpen;

    // 自动维护的瘤子列表
    //private List<PulsingTumor> allTumors = new List<PulsingTumor>();

    void Start()
    {
        // 🛡️ 自动全图搜索所有挂了 PulsingTumor 的物体
        //PulsingTumor[] foundTumors = FindObjectsByType<PulsingTumor>(FindObjectsSortMode.None);
        //allTumors.AddRange(foundTumors);

        // 认亲大会：让每个瘤子知道谁是老大
        //foreach (var tumor in allTumors)
        //{
        //    tumor.manager = this;
        //}

        //Debug.Log($"已连接 {allTumors.Count} 个肉瘤等待清理。");
    }

    // 每次有瘤子爆炸都会被调用
    public void CheckAllTumors()
    {
        bool allClear = true;
        int destroyedCount = 0;

        // 检查是不是都炸了
        //foreach (var tumor in allTumors)
        //{
        //    if (tumor.IsDestroyed())
        //    {
        //        destroyedCount++;
        //    }
        //    else
        //    {
        //        allClear = false; // 只要有一个没炸，就没完
        //    }
        //}

        //Debug.Log($"进度：{destroyedCount} / {allTumors.Count}");

        if (allClear)
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        Debug.Log("🎉 所有肉瘤清除完毕，通道开启！");
        if (doorToOpen != null)
        {
            // 这里可以换成播放动画，现在简单粗暴地关掉门
            doorToOpen.SetActive(false);
            // 播放一个成功的音效?
        }
    }
}