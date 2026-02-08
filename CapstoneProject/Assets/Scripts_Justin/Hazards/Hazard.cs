using UnityEngine;

/// <summary>
/// Simple script for handling the hazard's behavior
/// </summary>
public class Hazard : MonoBehaviour
{
    [SerializeField] private GameObject UIScreen;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collided with {other.gameObject}", other.gameObject);
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerController>().SendToCheckpoint();
            //TODO Play truck-hitting sound

            if(UIScreen != null)
                UIScreen.SetActive(true);

        }
        else if(other.gameObject.tag == "Enemy")
        {
            EntityManager manager = other.GetComponent<EntityManager>();
            if(manager.isAlive)
            {
                manager.EntityHasDied();
            }
        }
    }
}
