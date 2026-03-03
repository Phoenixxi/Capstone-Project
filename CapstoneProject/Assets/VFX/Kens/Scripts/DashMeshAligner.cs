using UnityEngine;

public class DashMeshAligner : MonoBehaviour
{
    // Pass the velocity from your character controller or Rigidbody
    public Vector3 currentVelocity;

    void Update()
    {
        // Prevent snapping to default rotation when velocity drops to 0
        if (currentVelocity.sqrMagnitude > 0.01f)
        {
            // Rotates the transform to face the velocity vector, position is untouched
            transform.rotation = Quaternion.LookRotation(currentVelocity.normalized);
        }
    }
}