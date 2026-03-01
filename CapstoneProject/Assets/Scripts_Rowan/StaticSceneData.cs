using UnityEngine;
using lilGuysNamespace;
using AmplifyShaderEditor;

public class StaticSceneData : MonoBehaviour
{
    public static bool playerReattempting = false;
    public static bool playerReachedBossZone = false;

    public void OnTriggerEnter(Collider other)
    {
        playerReachedBossZone = true;
    }
    }
