using UnityEngine;
using lilGuysNamespace;

public class HealthPack : MonoBehaviour
{
    // Serialized so that designers can adjust in the inspector
    [SerializeField] public float healAmount = 25f; // how much health to restore

    private void OnTriggerEnter(Collider other)  
    {
       EntityManager player = other.GetComponentInParent<EntityManager>();

        if (player != null)
        {
            player.Heal(healAmount);
            Destroy(gameObject); // remove the health pack after healing
        }
    }
}
