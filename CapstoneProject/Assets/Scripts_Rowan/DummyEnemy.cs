using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
   public float damageAmount = 5f; // how much health to restore

    private void OnCollisionEnter(Collision other)  
    {
       PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.TakeDamageWrapper(damageAmount);
        }
    }
}
