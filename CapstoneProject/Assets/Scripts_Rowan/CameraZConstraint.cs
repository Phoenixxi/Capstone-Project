using UnityEngine;
using Unity.Cinemachine;

public class CameraZConstraint : MonoBehaviour
{
    [SerializeField] private float minZPosition = -30f; 
    private CinemachineBrain brain;
    
    void Start()
    {
        brain = GetComponent<CinemachineBrain>();
    }
    
    void OnEnable()
    {
        CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    }
    
    void OnDisable()
    {
        CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);
    }
    
    private void OnCameraUpdated(CinemachineBrain brain)
    {
        if (brain != this.brain) 
            return;
        
        Vector3 currentPosition = transform.position;
        float newZPosition = Mathf.Max(currentPosition.z, minZPosition);
        
        if (currentPosition.z != newZPosition)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, newZPosition);
        }
    }


}
