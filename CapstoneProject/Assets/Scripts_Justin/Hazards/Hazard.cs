using UnityEngine;
using Steamworks;
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

            if (SteamClient.IsValid)
            {
                var achievement = new Steamworks.Data.Achievement("ACH_IsekaiTruck");
                if (!achievement.State)
                {
                    achievement.Trigger();
                    Destroy(gameObject);
                }
            }

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
