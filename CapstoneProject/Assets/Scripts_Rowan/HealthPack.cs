using UnityEngine;
using lilGuysNamespace;

public class HealthPack : MonoBehaviour
{
    // Serialized so that designers can adjust in the inspector
    [SerializeField] public float healAmount = 25f; // how much health to restore

    private void OnTriggerEnter(Collider other)  
    {
       PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            Debug.Log("heal called");
            player.HealActiveCharacter(healAmount);
            Destroy(gameObject); // remove the health pack after healing
        }
    }
}
