using System;
using UnityEngine;

/// <summary>
/// Ability that creates a zone on the ground that buffs the user and damages enemies
/// </summary>
public class BuffZoneAbility : Ability
{
    [Header("Buff Zone Ability Settings")]
    [SerializeField] private GameObject buffZonePrefab;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float duration;
    [SerializeField] private float damageRate;
    [SerializeField] private float attackCooldownMultiplier;
    //TODO Implement buffing component


    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse) return Array.Empty<AbilityMovement>();
        Vector3 spawnLocation = new Vector3(transform.position.x, transform.position.y + BuffZone.SPAWN_OFFSET, transform.position.z);
        BuffZone buffZone = Instantiate(buffZonePrefab, spawnLocation, Quaternion.identity).GetComponent<BuffZone>();
        buffZone.SetRadius(radius);
        buffZone.SetDamage(damage);
        buffZone.SetDamageRate(damageRate);
        buffZone.SetElement(element);
        buffZone.SetAttackCooldownMultiplier(attackCooldownMultiplier);
        buffZone.StartTimer(duration);
        currentCooldown = cooldown;
        return Array.Empty<AbilityMovement>();
    }


    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
