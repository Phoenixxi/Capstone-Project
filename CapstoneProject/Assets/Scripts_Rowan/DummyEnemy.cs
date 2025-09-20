using UnityEngine;

public class DummyEnemy : MonoBehaviour
{
   public float damageAmount = 5f; // how much health to restore

    private void OnCollisionEnter(Collision other)  
    {
        Debug.Log("in collision");
       PlayerController player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            Debug.Log("player not null");
            player.TakeDamageWrapper(damageAmount);
        }
    }
}
