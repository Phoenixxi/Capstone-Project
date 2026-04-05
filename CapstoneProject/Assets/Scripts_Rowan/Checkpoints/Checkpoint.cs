using UnityEngine;
using lilGuysNamespace;


public class Checkpoint : MonoBehaviour
{
    public bool isCheckpointReached = false; //Has the player made contact with this checkpoint?
    public bool isNewestCheckpoint = false; // Is this the furthest along checkpoint the player has crossed?
    public bool hasPlayerFallen = false;    // True after player falls off map
    [SerializeField] public CheckpointController checkpointController;

    public bool buildingStage;
    public bool preParkour;
    public bool postParkour;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCheckpointReached = false;
        isNewestCheckpoint = false;
    }

    //Check if player reaches checkpoint
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isCheckpointReached = true;

            CheckRestartLocations();
            
            if(hasPlayerFallen)
            {
                checkpointController.playerController.TakeDamageWrapper(20);
                hasPlayerFallen = false;
            }
        }

        checkpointController.UpdateCheckpointList(this);
    }

    public void CheckRestartLocations()
    {
        if(buildingStage)
            StaticSceneData.playerReachedBuildingStage = true;
        else if(preParkour)
            StaticSceneData.playerReachedPREparkourStage = true;
        else if(postParkour)
            StaticSceneData.playerReachedPOSTparkourStage = true;
    }
    
}
