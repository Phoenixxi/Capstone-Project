using UnityEngine;
using lilGuysNamespace;

public class Checkpoint : MonoBehaviour
{
    public bool isCheckpointReached = false; //Has the player made contact with this checkpoint?
    public bool isNewestCheckpoint = false; // Is this the furthest along checkpoint the player has crossed?
    [SerializeField] public CheckpointController checkpointController;

    [Header("Death Pit Info. Leave Blank if not a death pit")]
    public bool isDeathPit;
    [SerializeField] public PlayerController playerController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCheckpointReached = false;
        isNewestCheckpoint = false;

        if(isDeathPit)
        {
            if(playerController == null)
                Debug.Log("Player Controller for Death pit is not set in inspector");
        }
    }

    //Check if player reaches checkpoint
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checkpoint Triggered");
        if (other.CompareTag("Player"))
        {
            isCheckpointReached = true;

            if(isDeathPit){
                playerController.TakeDamageWrapper(20);
                return;
            }
            
            Debug.Log("Checkpoint reached");
        }

        checkpointController.UpdateCheckpointList(this);
    }
}
