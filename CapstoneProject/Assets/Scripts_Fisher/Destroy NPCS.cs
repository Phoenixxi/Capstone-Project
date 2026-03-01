using UnityEngine;

public class DestroyNPCS : MonoBehaviour
{
    [SerializeField] private GameObject targetToDestroy;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(targetToDestroy);
        }
    }
}