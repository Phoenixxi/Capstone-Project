using System.Linq.Expressions;
using Unity.Cinemachine;
using UnityEngine;

public class CameraChangeTrigger : MonoBehaviour
{
    [SerializeField]
    private CinemachineCamera camera;
    private CinemachineCamera lastCamera;

    #region Const Variables
    private const int HIGHEST_PRIORITY = 10;
    private const int LOWEST_PRIORITY = 0;
    #endregion
    void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        //Check whether or not the object that passed through is the player or not
        if (other.tag != "Player") return;

        //Find all the cameras in the world
        GameObject[] cameras = GameObject.FindGameObjectsWithTag("Camera");

        //loop through each camera in the world and set their priority to 0 (lowest priority)
        foreach (GameObject cam in cameras)
        {
            CinemachineCamera cinemachineCamera;
            cam.TryGetComponent<CinemachineCamera>(out cinemachineCamera);

            //make the camera the lowest priority 
            cinemachineCamera.Priority = LOWEST_PRIORITY;
        }

        //change this camera to be the highest priority
        camera.Priority = HIGHEST_PRIORITY;
    }
}
