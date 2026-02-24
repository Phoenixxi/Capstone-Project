using System.Collections.Generic;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

public enum LayerMaskType { PlayerLayerMask, EnemyLayerMask } 

/// <summary>
/// Handles colliders that damage enemies during melee combat
/// </summary>
public class Hurtbox : MonoBehaviour
{
    [SerializeField] private float screenShakeIntensity;
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private LayerMaskType layerMaskType;
    [SerializeField] private bool causesKnockback = false;
    [SerializeField] private bool orientationAffectsKnockback = true;
    [SerializeField] private float knockbackDuration = 0f;
    [SerializeField] private Vector3 knockback = Vector3.zero;

    private bool hasShakenScreen = false;
    private float activeTime;
    private float currentActiveTime;
    private int damage;
    private Collider meleeCollider;
    private ElementType element;
    private CameraController cameraController;
    private LayerMask layerMask;
    private bool isDashing;
    private HashSet<EntityManager> hitEntities;

    private void Awake()
    {
        isDashing = false;
        meleeCollider = GetComponent<Collider>();
        meleeCollider.enabled = false;
        enabled = false;
        cameraController = FindFirstObjectByType<CameraController>();

        switch (layerMaskType)
        {
            default:
                layerMask = LayerMask.GetMask("Enemy");
                break;
            case LayerMaskType.EnemyLayerMask:
                layerMask = LayerMask.GetMask("Player");
                break;
        }
    }

    private void OnEnable()
    {
        currentActiveTime = 0f;
        hitEntities = new HashSet<EntityManager>();
        meleeCollider.enabled = true;
        HitDetection();
        Debug.Log("Hurtbox activated");
    }

    private void OnDisable()
    {
        Debug.Log("Hurtbox deactivated");
        meleeCollider.enabled = false;
        hasShakenScreen = false;
    }

    private void Update()
    {
        currentActiveTime += Time.deltaTime;
        if (currentActiveTime >= activeTime) enabled = false;
    }

    /// <summary>
    /// Turns this hurtbox on for a given amount of time
    /// </summary>
    /// <param name="activeTime">The time this hurtbox should be turned on for</param>
    public void Activate(float activeTime, bool dash)
    {
        isDashing = dash;
        enabled = true;
        hitEntities.Clear();
        this.activeTime = activeTime;
    }

    /// <summary>
    /// Sets how much damage this hurtbox should do on a collision
    /// </summary>
    /// <param name="damage"></param>
    public void SetHurtboxDamage(int damage)
    {
        this.damage = damage;
    }

    public void SetElementType(ElementType type)
    {
        element = type;
    }

    private void DamageEntity(EntityManager entity)
    {
        if (!causesKnockback) entity.TakeDamage(damage, element);
        else
        {
            KnockbackMovement movement;
            if(orientationAffectsKnockback)
            {
                Vector3 baseDirection = transform.forward;
                Vector3 forwardDirection = baseDirection * knockback.x;
                Vector3 upDirection = Quaternion.AngleAxis(90f, Vector3.right) * baseDirection * knockback.y;
                Vector3 sideDirection = Quaternion.AngleAxis(90f, Vector3.up) * baseDirection * knockback.z;
                Vector3 finalDirection = (baseDirection + upDirection + sideDirection) * knockback.magnitude;
                movement = new KnockbackMovement(finalDirection, Time.time, knockbackDuration);
            } else
            {
                movement = new KnockbackMovement(knockback, Time.time, knockbackDuration);
            }
            entity.TakeDamage(damage, element, movement);
        }
        hitEntities.Add(entity);
    }

    //dash for Zoom (CHANGE CODE IF OTHER CHARACTERS USE MELEE)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{other.gameObject} in trigger");
        if (!isDashing) return;
        Debug.Log($"Melee hit {other.gameObject}", other.gameObject);
        EntityManager hitEntity = other.gameObject.GetComponent<EntityManager>();
        if (hitEntity == null) hitEntity = other.gameObject.GetComponentInChildren<EntityManager>();
        Debug.Log($"Entity manager null: {hitEntity == null}");
        Debug.Log($"Entity manager in hit entities: {hitEntities.Contains(hitEntity)}");
        if (hitEntity == null || hitEntities.Contains(hitEntity)) return;
        DamageEntity(hitEntity);
        if (!hasShakenScreen)
        {
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
            hasShakenScreen = true;
        }
    }

    private void HitDetection()
    {
        if(isDashing) return;
        Collider[] colliders = Physics.OverlapBox(meleeCollider.bounds.center, meleeCollider.bounds.extents, transform.rotation, layerMask);

        foreach (Collider collider in colliders)
        {
            Debug.Log($"Melee hit {collider.gameObject}", collider.gameObject);
            EntityManager entityManager = collider.gameObject.GetComponentInChildren<EntityManager>();
            if (entityManager == null) entityManager = collider.gameObject.GetComponent<EntityManager>();
            if (entityManager == null)
            {
                Debug.LogError("oh thats not good brother, there should be an entitymanager");
            }
            else if (hitEntities.Contains(entityManager)) return;
            DamageEntity(entityManager);
            //entityManager.TakeDamage(damage, element);
            //hitEntities.Add(entityManager);
        }

        if(!hasShakenScreen)
        {
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
            hasShakenScreen = true;
        }
    }
}
 