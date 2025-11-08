using UnityEngine;
using System.Collections.Generic;
using lilGuysNamespace;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] public PlayerController playerController;
    [SerializeField] public List<Checkpoint> checkpointList;
    private Checkpoint mostRecentCheckpoint;

    void Start()
    {
        // Make sure list is not null on start
        if(checkpointList.Count > 0)
            mostRecentCheckpoint = checkpointList[0];

        if(playerController == null)
            Debug.LogError("Player Controller not set in Checkpoint controller");
    }

    public void UpdateCheckpointList(Checkpoint cp)
    {
        // If this is the first time the checkpoint has been reached
        if(!cp.isNewestCheckpoint)
        {
            cp.isNewestCheckpoint = true;
            mostRecentCheckpoint = cp;
        }
    }

    // Give location of most recent checkpoint to player if they fell
    public Vector3 RecentCheckpointLocation()
    {
        Debug.Log("CP Location: " + mostRecentCheckpoint.transform.position);
        mostRecentCheckpoint.hasPlayerFallen = true;
        return mostRecentCheckpoint.transform.position;
    }
}
