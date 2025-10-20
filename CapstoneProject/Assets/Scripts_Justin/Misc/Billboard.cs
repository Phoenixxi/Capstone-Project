using UnityEngine;

/// <summary>
/// This class forces 2D sprites to face towards the camera
/// </summary>
public class Billboard : MonoBehaviour
{
    Transform cameraTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraTransform);
    }
}
