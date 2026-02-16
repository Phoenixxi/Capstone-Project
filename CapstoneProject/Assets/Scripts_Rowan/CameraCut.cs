using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CameraCut : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject cutSceneCamera;
    private PlayerInput playerInput;

    void Start()
    {
        GameObject Player = GameObject.Find("Player");
        playerInput = Player.GetComponent<PlayerInput>();
    }

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
            StartCoroutine(waitTime());
        }
    }

    IEnumerator waitTime()
    {
        /// wait 2.5 seconds
        yield return new WaitForSeconds(2.5f);
        // reset cameras
        mainCamera.SetActive(true);
        cutSceneCamera.SetActive(false); 
        StartCoroutine(onReturn());
        
    }

    // Gives player controls back
    IEnumerator onReturn()
    {
        yield return new WaitForSeconds(1.8f);
        playerInput.actions.FindActionMap("Player").Enable();
        // Destoy this trigger
        Destroy(gameObject);
    }
}
