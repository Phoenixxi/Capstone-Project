using UnityEngine;

public class CameraXConstraintTrigger : MonoBehaviour
{
    [SerializeField] private float triggerMinX = -30f;
    [SerializeField] private CameraXConstraint cameraConstraint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraConstraint.ActivateConstraint(triggerMinX);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraConstraint.DeactivateConstraint();
    }

    private void OnDestroy()
    {
        cameraConstraint.DeactivateConstraint();
    }
}