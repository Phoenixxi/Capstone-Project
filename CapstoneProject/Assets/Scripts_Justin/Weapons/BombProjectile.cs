using UnityEngine;

/// <summary>
/// Special type of Projectile that has physics and explodes
/// </summary>
public class BombProjectile : Projectile
{
    [SerializeField] protected LayerMask validCollisions; //Defines what will cause the bomb to explode, not who will be damaged by them
    [SerializeField] protected LayerMask explosionTargets;
    [SerializeField] protected float explosionRadius = 3;
    [SerializeField] protected float innerScreenShakeRadius; //Distancec at which the player will receive the full screen shake amount
    [SerializeField] protected float outerScreenShakeRadius; //The maximum distance away from the bomb the player can be to still get screenshot; screen shake strength decreases with distance
    //TODO Add fields for VFX and SFX

    protected override void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= projectileLifetime) Explode();
    }

    public override void ChangeMoveDirection(Vector3 newDirection)
    {
        //base.ChangeMoveDirection(newDirection);
        projectile.AddForce(newDirection * projectileSpeed, ForceMode.Impulse);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        OnCollide(other);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        OnCollide(collision.collider);
    }

    /// <summary>
    /// Helper method that handles both trigger and collision enter calls. Checks to see if the collision should trigger
    /// an explosion and does so if yes
    /// </summary>
    /// <param name="other">The other object the bomb collided with</param>
    private void OnCollide(Collider other)
    {
        Debug.Log($"Bomb collided with {other.gameObject}");
        int otherLayerMask = 1 << other.gameObject.layer;
        //Checking if collider layer is within the accepted list of layers
        if((validCollisions & otherLayerMask) != 0) {
            Explode();
        }
    }

    /// <summary>
    /// Makes the bomb explode and damage targets around it
    /// </summary>
    private void Explode()
    {
        //TODO Add SFX and VFX
        Ray explosionRay = new Ray(transform.position, Vector3.down);
        RaycastHit[] hitTargets = Physics.SphereCastAll(explosionRay, explosionRadius, 0.1f, explosionTargets);
        foreach(RaycastHit hitTarget in hitTargets)
        {
            EntityManager hitEntity = hitTarget.transform.GetComponent<EntityManager>();
            if (hitEntity == null) hitEntity = hitTarget.transform.GetComponentInChildren<EntityManager>();
            if(hitEntity != null)
            {
                hitEntity.data = data;  //Sends the DOT data to entity's manager
                hitEntity.TakeDamage(damage, elementType);
            }
        }
        //cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
        ShakeScreen();
        Destroy(gameObject);
    }

    private void ShakeScreen()
    {
        PlayerController player = FindFirstObjectByType<PlayerController>();
        Vector3 playerPos = player.transform.position;
        float explosionDistance = Vector3.Distance(playerPos, transform.position);
        if(explosionDistance <= innerScreenShakeRadius)
        {
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
            Debug.Log("Received max screen shake");
        } else if(explosionDistance <= outerScreenShakeRadius)
        {
            float adjustedRadius = outerScreenShakeRadius - innerScreenShakeRadius;
            float adjustedDistance = explosionDistance - innerScreenShakeRadius;
            float intensityPercentage = 1 - (adjustedDistance / adjustedRadius);
            cameraController.ShakeCamera(screenShakeIntensity * intensityPercentage, screenShakeDuration);
            Debug.Log($"Received {intensityPercentage} percent screen shake");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
        Gizmos.DrawSphere(transform.position, explosionRadius);
    }
}
