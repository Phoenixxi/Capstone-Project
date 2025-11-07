using UnityEngine;
using lilGuysNamespace;

public class Checkpoint : MonoBehaviour
{
    public bool isCheckpointReached = false; //Has the player made contact with this checkpoint?
    public bool isNewestCheckpoint = false; // Is this the furthest along checkpoint the player has crossed?
    [SerializeField] public CheckpointController checkpointController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCheckpointReached = false;
        isNewestCheckpoint = false;
    }

    //Check if player reaches checkpoint
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checkpoint Triggered");
        if (other.CompareTag("Player"))
        {
            isCheckpointReached = true;
            Debug.Log("Checkpoint reached");
        }

        checkpointController.UpdateCheckpointList(this);
    }
}
