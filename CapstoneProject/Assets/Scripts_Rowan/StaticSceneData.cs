using UnityEngine;
using lilGuysNamespace;

public class StaticSceneData : MonoBehaviour
{
    public static bool playerReattempting = false;
    public static bool playerReachedBossZone = false;

    public static bool playerReachedLevel2 = false;

    public static bool playerReachedBuildingStage = false;
    public static bool playerReachedPREparkourStage = false;
    public static bool playerReachedPOSTparkourStage = false;

    public void OnTriggerEnter(Collider other)
    {
        playerReachedBossZone = true;
    }

}
