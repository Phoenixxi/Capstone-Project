using UnityEngine;
using Unity.Cinemachine;

public class CameraXConstraint : MonoBehaviour
{
    private float maxXPosition;
    private CinemachineBrain brain;
    private bool constraintActive = false;

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
        if (!constraintActive) return;
        if (brain != this.brain)
            return;

        Vector3 currentPosition = transform.position;
        float newXPosition = Mathf.Min(currentPosition.x, maxXPosition);

        if (currentPosition.x != newXPosition)
        {
            transform.position = new Vector3(newXPosition, currentPosition.y, currentPosition.z);
        }
    }

    public void ActivateConstraint(float newMaxX)
    {
        maxXPosition = newMaxX;
        constraintActive = true;
    }

    public void DeactivateConstraint()
    {
        constraintActive = false;
    }
}