using UnityEngine;
using Steamworks;
public class BeatLevel2Trigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (SteamClient.IsValid)
            {
                var achievement = new Steamworks.Data.Achievement("ACH_BeatLevel2");
                if (!achievement.State)
                {
                    achievement.Trigger();
                    Destroy(gameObject);
                }
            }
        }
    }
}