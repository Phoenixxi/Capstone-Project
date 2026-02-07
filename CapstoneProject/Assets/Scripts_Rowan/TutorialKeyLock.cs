using UnityEngine;
using System.Collections.Generic;
using lilGuysNamespace;

public class TutorialKeyLock : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            playerController.DecrementKeyCount();

            Destroy(gameObject);
        }
    }
}
