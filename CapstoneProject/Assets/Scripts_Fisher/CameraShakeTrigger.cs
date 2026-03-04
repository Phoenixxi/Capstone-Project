using UnityEngine;

public class CameraShakeTrigger : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;
    [SerializeField] private float intensity = 2f;
    [SerializeField] private float duration = 0.3f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            cameraController.ShakeCamera(intensity, duration);
        }
    }

    private void OnDestroy()
    {
        if (cameraController != null)
            cameraController.StopShake();
    }
}