using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
   public float damageAmount = 5f; // how much health to restore

    private void OnTriggerEnter(Collider other)  
    {
       EntityManager player = other.GetComponentInParent<EntityManager>();

        if (player != null)
        {
            player.TakeDamage(damageAmount);
        }
    }
}
