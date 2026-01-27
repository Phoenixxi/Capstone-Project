using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class HealthCameraCut : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
    [SerializeField] private GameObject cutSceneCamera;
    private PlayerInput playerInput;
    private GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
        playerInput = Player.GetComponent<PlayerInput>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            mainCamera.SetActive(false);
            cutSceneCamera.SetActive(true);
            playerInput.actions.FindActionMap("Player").Disable();
            Player.SetActive(false);
            StartCoroutine(waitTime());
        }
    }

    IEnumerator waitTime()
    {
        /// wait ___ seconds
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
        Player.SetActive(true);
        playerInput.actions.FindActionMap("Player").Enable();
        // Destoy this trigger
        Destroy(gameObject);
    }
}
