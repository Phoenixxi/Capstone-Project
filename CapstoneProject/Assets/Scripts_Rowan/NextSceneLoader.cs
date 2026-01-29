using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class NextSceneLoader : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "IntroCutscene")
            SceneManager.LoadScene("LoadingScreen");

        if(currentScene.name == "LoadingScreen")
        {
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadAsyncScene()
    {
        yield return new WaitForSeconds(3f);
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync("Level01_LouieScene");

        while(!loadOperation.isDone)
        {
            yield return null;
        }
    }




}