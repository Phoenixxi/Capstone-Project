using UnityEngine;
using UnityEngine.Splines;

public class PipeForcePass : MonoBehaviour
{
    public SplineContainer spline; // Drag your spline object here
    public float speed = 5f;
    private float distancePercentage = 0f;
    private bool isPassing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPassing)
        {
            isPassing = true;
            StartCoroutine(FollowSpline(other.transform));
        }
    }

    private System.Collections.IEnumerator FollowSpline(Transform player)
    {
        // Disable player control
        var cc = player.GetComponent<CharacterController>();
        if (cc) cc.enabled = false;

        distancePercentage = 0;
        float splineLength = spline.CalculateLength();

        while (distancePercentage < 1f)
        {
            distancePercentage += (speed * Time.deltaTime) / splineLength;

            // Get position and rotation from spline directly
            player.position = spline.EvaluatePosition(distancePercentage);
            player.forward = spline.EvaluateTangent(distancePercentage);

            yield return null;
        }

        if (cc) cc.enabled = true;
        isPassing = false;
    }
}
