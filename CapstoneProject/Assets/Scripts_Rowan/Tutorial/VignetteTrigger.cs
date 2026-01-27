using UnityEngine;

public class VignetteTrigger : MonoBehaviour
{
    [SerializeField] private GameObject vignetteCanvas;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            vignetteCanvas.SetActive(true);
            Destroy(gameObject);
        }
    }
}
