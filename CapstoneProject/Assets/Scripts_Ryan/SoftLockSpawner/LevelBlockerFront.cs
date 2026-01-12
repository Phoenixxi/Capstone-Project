using UnityEngine;

public class LevelBlockerFront : MonoBehaviour
{
    [SerializeField] private LevelBlocker levelBlocker;

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            levelBlocker.initializeMobs();
        }

        Destroy(gameObject);
    }
}
