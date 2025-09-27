using UnityEngine;
using lilGuysNamespace;

/// <summary>
/// Handles behavior for projectiles fired by the player and enemies
/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float projectileLifetime;

    
    private Rigidbody projectile;
    private int damage;
    private EntityData.ElementType elementType = EntityData.ElementType.Normal;
    private float currentLifetime;


    private void Awake()
    {
        projectile = GetComponent<Rigidbody>();
        currentLifetime = 0f;
    }

    private void Update()
    {
        currentLifetime += Time.deltaTime;
        if (currentLifetime >= projectileLifetime) Destroy(gameObject);
    }

    /// <summary>
    /// Sets the new movement direction for this projectile
    /// </summary>
    /// <param name="newDirection">The direction the projectile should move in. Note that the vector is expected to be normalized</param>
    public void ChangeMoveDirection(Vector3 newDirection)
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
        Debug.Log("Element set: " + element.ToString());
        this.elementType = element;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        EntityManager hitEntity = collision.gameObject.GetComponent<EntityManager>();
        if (hitEntity != null) hitEntity.TakeDamage(damage, elementType);
        Destroy(gameObject);
    }

}
