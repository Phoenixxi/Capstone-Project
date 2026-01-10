using UnityEngine;
using lilGuysNamespace;

/// <summary>
/// Handles behavior for projectiles fired by the player and enemies
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileLifetime;

    [SerializeField] protected AbilityData data;
    [SerializeField] protected float screenShakeIntensity;
    [SerializeField] protected float screenShakeDuration;

    protected Rigidbody projectile;
    protected int damage;
    protected EntityData.ElementType elementType = EntityData.ElementType.Normal;
    protected float currentLifetime;
    protected CameraController cameraController;


    protected virtual void Awake()
    {
        projectile = GetComponent<Rigidbody>();
        currentLifetime = 0f;
        cameraController = FindFirstObjectByType<CameraController>();
    }

    protected virtual void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= projectileLifetime) Destroy(gameObject);
    }

    /// <summary>
    /// Sets the new movement direction for this projectile
    /// </summary>
    /// <param name="newDirection">The direction the projectile should move in. Note that the vector is expected to be normalized</param>
    public virtual void ChangeMoveDirection(Vector3 newDirection)
    {
        projectile.linearVelocity = newDirection * projectileSpeed;
    }

    /// <summary>
    /// Sets the projectile's damage value
    /// </summary>
    /// <param name="damage"></param>
    public void SetProjectileDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetProjectileElement(EntityData.ElementType element)
    {
        //Debug.Log("Element set: " + element.ToString());
        this.elementType = element;
        
    }

    protected virtual void OnTriggerEnter(UnityEngine.Collider other)
    {
        Debug.Log($"Hit {other}", other);
        EntityManager hitEntity = other.gameObject.GetComponent<EntityManager>();
        if (hitEntity == null) hitEntity = other.gameObject.GetComponentInChildren<EntityManager>();
        if (hitEntity != null) {
            hitEntity.data = data;  //Sends the DOT data to entity's manager
            hitEntity.TakeDamage(damage, elementType);
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
        }

        Destroy(gameObject);

    }

}
