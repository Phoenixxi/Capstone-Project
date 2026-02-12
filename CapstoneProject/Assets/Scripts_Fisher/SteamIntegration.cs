using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamIntegration : MonoBehaviour
{
    
    void Start()
    {
        try
        {
            Steamworks.SteamClient.Init(4343350);
            PrintYourName();
            UnlockOpenGameAchievement(); 
            Debug.Log("It worked lmao");
        }
        catch (System.Exception e)
        {
            // Something went wrong it can be:
            // Steam is closed?
            //Cant find steam_api dll?
            //Dont have permission to play app?
            
            Debug.Log("It no work lmao");
        }
    }

    private void PrintYourName()
    {
        Debug.Log(Steamworks.SteamClient.Name);
    }

    void Update()
    {
        Steamworks.SteamClient.RunCallbacks();
    }
    private void UnlockOpenGameAchievement()
    {
        var achievement = new Steamworks.Data.Achievement("ACH_OpenGame");

        if (!achievement.State)
        {
            achievement.Trigger();
            Debug.Log("Achievement Unlocked!");
        }
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }
}
