using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    private AudioManager audioManager;


    private void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
    }
    public void PlayGame()
    {
        StaticSceneData.playerReattempting = false;
        StaticSceneData.playerReachedBossZone = false;
        PlayButtonSound();
        SceneManager.LoadScene("IntroCutscene");
    }


    public void QuitGame()
    {
        Debug.Log("QUIT!");
        PlayButtonSound();
        Application.Quit();
    }


    public void ReturnGame()
    {
        PlayButtonSound();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }


    public void ReturnMenu()
    {
        PlayButtonSound();
        SceneManager.LoadScene("MainMenu");
    }

    public void ReattemptLevel()
    {
        PlayButtonSound();
        StaticSceneData.playerReattempting = true;
        SceneManager.LoadScene("LoadingScreen");
    }

    public void PlayButtonSound()
    {
        audioManager.PlaySound(SoundName.BUTTON_CLICK);
    }


}
