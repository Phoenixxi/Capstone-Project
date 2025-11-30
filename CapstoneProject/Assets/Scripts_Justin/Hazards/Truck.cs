using UnityEngine;

/// <summary>
/// Simple script for handling the truck hazard's behavior
/// </summary>
public class Truck : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with {other.gameObject}", other.gameObject);
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().SendToCheckpoint();
            //TODO Play truck-hitting sound
        }
    }
}
