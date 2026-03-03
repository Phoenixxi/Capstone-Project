using UnityEngine;

public class NPCTrigger : MonoBehaviour
{
    [SerializeField] private TurnOn[] targets;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (TurnOn target in targets)
            {
                if (target != null)
                    target.EnableObject();
            }
        }
    }
}