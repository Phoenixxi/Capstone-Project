using UnityEngine;

public class RevivePack : MonoBehaviour
{
    // Serialized so that designers can adjust in the inspector
    [SerializeField] public float healAmount = 10f; // how much health to restore

    private void OnTriggerEnter(Collider other)  
    {
       PlayerController player = other.GetComponentInParent<PlayerController>();

        if (player != null)
        {
            player.HealAllCharacters(healAmount);
            Destroy(gameObject); // remove the health pack after healing
        }
    }
}
