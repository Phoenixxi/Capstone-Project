using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void PlayGame()
    {
        StaticSceneData.playerReattempting = false;
        StaticSceneData.playerReachedBossZone = false;
        SceneManager.LoadScene("IntroCutscene");
    }


    public void QuitGame()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }


    public void ReturnGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void ReturnMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void ReattemptLevel()
    {
        if(StaticSceneData.playerReachedBossZone)
        StaticSceneData.playerReattempting = true;
        SceneManager.LoadScene("LoadingScreen");
    }


}
