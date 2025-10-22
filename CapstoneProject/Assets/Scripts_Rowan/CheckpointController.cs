using UnityEngine;
using System.Collections.Generic;
using lilGuysNamespace;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] public List<Checkpoint> checkpointList;
    private Checkpoint mostRecentCheckpoint;

    void Start()
    {
        // Make sure list is not null on start
        if(checkpointList.Count > 0)
            mostRecentCheckpoint = checkpointList[0];
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
        return mostRecentCheckpoint.transform.position;
    }
}
