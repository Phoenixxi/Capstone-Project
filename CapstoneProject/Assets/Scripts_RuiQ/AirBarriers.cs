using UnityEngine;

public class AirWall : MonoBehaviour
{
    // This script will run as soon as the game starts
    void Start()
    {
        // Get the MeshRenderer component attached to this object
        MeshRenderer wallRenderer = GetComponent<MeshRenderer>();

        // Check if the component exists to prevent null reference errors
        if (wallRenderer != null)
        {
            // Disable the renderer so the object becomes invisible in-game
            // But the Collider component will still stay active for physics
            wallRenderer.enabled = false;
        }
        else
        {
            Debug.LogWarning("AirWall script is attached to " + gameObject.name + " but no MeshRenderer was found.");
        }
    }
}