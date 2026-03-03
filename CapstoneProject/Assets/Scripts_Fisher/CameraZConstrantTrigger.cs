using UnityEngine;

public class CameraZConstrantTrigger : MonoBehaviour
{
    [SerializeField] private float triggerMinZ = -30f;
    [SerializeField] private CameraZConstraint cameraConstraint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraConstraint.ActivateConstraint(triggerMinZ);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            cameraConstraint.DeactivateConstraint();
    }
}