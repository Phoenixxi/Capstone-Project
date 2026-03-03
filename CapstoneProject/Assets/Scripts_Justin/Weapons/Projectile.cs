using UnityEngine;
using lilGuysNamespace;
using System;

/// <summary>
/// Handles behavior for projectiles fired by the player and enemies
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float projectileSpeed;
    [SerializeField] protected float projectileLifetime;
    [SerializeField] protected bool causesKnockback = false;
    [SerializeField] protected bool orientationAffectsKnockback = true; //Determines whether the raw knockback vector will be used or if it'll change based on projectile direction
    [SerializeField] protected Vector3 knockback = Vector3.zero;
    [SerializeField] protected float knockbackDuration = 0f;

    [SerializeField] protected AbilityData data;
    [SerializeField] protected float screenShakeIntensity;
    [SerializeField] protected float screenShakeDuration;

    protected Rigidbody projectile;
    protected int damage;
    protected EntityData.ElementType elementType = EntityData.ElementType.Normal;
    protected float currentLifetime;
    protected CameraController cameraController;

    public Action<int> OnProjectileHitEntity;


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
            if(causesKnockback)
            {
                KnockbackMovement movement;
                if(orientationAffectsKnockback)
                {
                    //Quaternion projectileRotation = Quaternion.FromToRotation(knockback.normalized, projectile.linearVelocity.normalized);
                    //Vector3 direction = projectileRotation * knockback;
                    Vector3 baseDirection = projectile.linearVelocity.normalized;
                    Vector3 forwardDirection = baseDirection * knockback.x;
                    Vector3 upDirection = Quaternion.AngleAxis(90f, Vector3.right) * baseDirection * knockback.y;
                    Vector3 sideDirection = Quaternion.AngleAxis(90f, Vector3.up) * baseDirection * knockback.z;
                    Vector3 finalDirection = (baseDirection + upDirection + sideDirection) * knockback.magnitude;
                    movement = new KnockbackMovement(finalDirection, Time.time, knockbackDuration);
                } else
                {
                    movement = new KnockbackMovement(knockback, Time.time, knockbackDuration);
                }
                hitEntity.TakeDamage(damage, elementType, movement);

            } else
            {
                hitEntity.TakeDamage(damage, elementType);
            }
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
            OnProjectileHitEntity?.Invoke(damage);
        }
        Destroy(gameObject);

    }

}
