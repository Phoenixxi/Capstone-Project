using UnityEngine;
using System.Collections.Generic;

public class TumorManager : MonoBehaviour
{
    [Header("目标门")]
    public GameObject doorToOpen;
    //private List<PulsingTumor> allTumors = new List<PulsingTumor>();

    void Start()
    {
        //PulsingTumor[] foundTumors = FindObjectsByType<PulsingTumor>(FindObjectsSortMode.None);
        //allTumors.AddRange(foundTumors);

        // 
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
        if (doorToOpen != null)
        {
            // 这里可以换成播放动画，现在简单粗暴地关掉门
            doorToOpen.SetActive(false);
            // 播放一个成功的音效?
        }
    }
}