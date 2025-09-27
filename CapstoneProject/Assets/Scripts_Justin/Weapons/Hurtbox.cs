using System.Collections.Generic;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Handles colliders that damage enemies during melee combat
/// </summary>
public class Hurtbox : MonoBehaviour
{
    private float activeTime;
    private float currentActiveTime;
    private HashSet<EntityManager> hitEntities;
    private int damage;
    private Collider meleeCollider;
    private ElementType element;

    private void Awake()
    {
        meleeCollider = GetComponent<Collider>();
        meleeCollider.enabled = false;
        enabled = false;
    }

    private void OnEnable()
    {
        currentActiveTime = 0f;
        hitEntities = new HashSet<EntityManager>();
        meleeCollider.enabled = true;
        Debug.Log("Hurtbox activated");
    }

    private void OnDisable()
    {
        Debug.Log("Hurtbox deactivated");
        meleeCollider.enabled = false;
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
    public void Activate(float activeTime)
    {
        enabled = true;
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


    // Melee for Zoom (CHANGE CODE IF OTHER CHARACTERS USE MELEE)
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Melee hit {other.gameObject}", other.gameObject);
        EntityManager hitEntity = other.gameObject.GetComponent<EntityManager>();
        if (hitEntity == null || hitEntities.Contains(hitEntity)) return;
        hitEntities.Add(hitEntity);
        hitEntity.TakeDamage(damage, ElementType.Zoom);    
    }
}
 