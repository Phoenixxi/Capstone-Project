using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CameraCut : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject cutSceneCamera;
    [SerializeField] private GameObject videoPlayer;
    [SerializeField] private GameObject fadeCanvas;
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private GameObject skipTutorialButton;
    private PlayerInput playerInput;
    private CanvasGroup fadeCanvasGroup;

    void Start()
    {
        GameObject Player = GameObject.Find("Player");
        playerInput = Player.GetComponent<PlayerInput>();
        if(fadeCanvas != null)
            fadeCanvasGroup = fadeCanvas.GetComponent<CanvasGroup>();
    }

// Moves to second camera
    public void OnTriggerEnter(Collider other)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Level2-Rework")
        {
           //mainCamera.SetActive(false);
            cutSceneCamera.SetActive(true);
            Destroy(gameObject);
        }
        else
        {
            
            mainCamera.SetActive(false);
            cutSceneCamera.SetActive(true);
            playerInput.actions.FindActionMap("Player").Disable();
            StartCoroutine(waitTime(2.5f));
            
        }
    }

    IEnumerator waitTime(float time)
    {
        /// wait 2.5 seconds
        yield return new WaitForSeconds(time);
        // reset cameras
        if(gameObject.name == "TutorialCutReactionsTrigger") // zoom in for reactions cutscene
        {
            fadeCanvas.SetActive(true);
            // turn alpha up on canvas to make it fade to white using delta time
            while(fadeCanvasGroup.alpha < 1)
            {
                fadeCanvasGroup.alpha += Time.deltaTime;
                yield return null;
            }
            videoPlayer.SetActive(true);
            HUDCanvas.SetActive(false);
            StartCoroutine(loadingTime());
            StartCoroutine(videoWaitTime(18.8f)); // length of video in seconds
        }
        else
        {
            mainCamera.SetActive(true);
            cutSceneCamera.SetActive(false); 
            StartCoroutine(onReturn());
        }
        
    }
    

    //Gives game half a second to load everything properly
    IEnumerator loadingTime()
    {
        yield return new WaitForSeconds(0.5f);
        skipTutorialButton.SetActive(true);
        fadeCanvas.SetActive(false);
    }

    IEnumerator videoWaitTime(float time)
    {
        // wait for length of video
        mainCamera.SetActive(true);
        cutSceneCamera.SetActive(false); 
        yield return new WaitForSeconds(time);
        // reset cameras
        videoPlayer.SetActive(false);
        HUDCanvas.SetActive(true);
        skipTutorialButton.SetActive(false);
        playerInput.actions.FindActionMap("Player").Enable();
        Destroy(gameObject);
    }

    // Gives player controls back  
    IEnumerator onReturn()
    {
        yield return new WaitForSeconds(1.8f);
        playerInput.actions.FindActionMap("Player").Enable();
        // Destoy this trigger
        Destroy(gameObject);
    }

    public void SkipTutorialVideo()
    {
        StopAllCoroutines();
        mainCamera.SetActive(true);
        cutSceneCamera.SetActive(false); 
        videoPlayer.SetActive(false);
        HUDCanvas.SetActive(true);
        skipTutorialButton.SetActive(false);
        playerInput.actions.FindActionMap("Player").Enable();
        Destroy(gameObject);
    }
}
