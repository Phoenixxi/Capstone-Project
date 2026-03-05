using UnityEngine;

public class DestroyNPCS : MonoBehaviour
{
    [SerializeField] private GameObject[] targetsToDestroy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (GameObject target in targetsToDestroy)
            {
                if (target != null)
                {
                    Destroy(target);
                }
            }
        }
    }
}