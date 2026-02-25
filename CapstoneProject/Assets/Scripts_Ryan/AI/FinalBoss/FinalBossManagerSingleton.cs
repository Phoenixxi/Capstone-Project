using System;
using UnityEngine;

public class FinalBossManagerSingleton : MonoBehaviour
{
    public static FinalBossManagerSingleton Instance {get; private set;}
    public Action InitializeFinalBoss;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeBoss()
    {
        InitializeFinalBoss?.Invoke();
    }
}
