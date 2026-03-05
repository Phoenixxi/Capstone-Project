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
        }
        catch (System.Exception e)
        {
            // Something went wrong it can be:
            // Steam is closed?
            //Cant find steam_api dll?
            //Dont have permission to play app?
            
        }
    }

    private void PrintYourName()
    {

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
        }
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }


    public void IsThisAchievementUnlocked(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);

    }

    public void UnlockAchievement(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Trigger();
    }

    public void ClearAchievementStatus(string id)
    {
        var ach = new Steamworks.Data.Achievement(id);
        ach.Clear();
    }
}

