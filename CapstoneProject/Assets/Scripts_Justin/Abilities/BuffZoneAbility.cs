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
    [SerializeField] private float movementFreezeTime;
    [SerializeField] public Animator animator;

    private float currentFreezeTimer = 0f;


    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse) return Array.Empty<AbilityMovement>();
        abilityInUse = true;
        animator.SetTrigger("UseAbility");
        currentFreezeTimer = 0f;
        movements[0] = new AbilityMovement(Vector3.zero);
        return movements;
    }


    protected override void Update()
    {
        base.Update();
        if(abilityInUse)
        {
            currentFreezeTimer += Time.deltaTime;
            if(currentFreezeTimer >= movementFreezeTime)
            {
                abilityInUse = false;
                movements[0].Complete();
                SpawnBuffZone();
                currentCooldown = cooldown;
            }
        }
    }

    /// <summary>
    /// Spawns the ability buff zone and sets its properties
    /// </summary>
    private void SpawnBuffZone()
    {
        Vector3 spawnLocation = new Vector3(transform.position.x, transform.position.y + BuffZone.SPAWN_OFFSET, transform.position.z);
        BuffZone buffZone = Instantiate(buffZonePrefab, spawnLocation, Quaternion.identity).GetComponent<BuffZone>();
        buffZone.SetRadius(radius);
        buffZone.SetDamage(damage);
        buffZone.SetDamageRate(damageRate);
        buffZone.SetElement(element);
        buffZone.SetAttackCooldownMultiplier(attackCooldownMultiplier);
        buffZone.StartTimer(duration);
    }

    protected override void Awake()
    {
        base.Awake();
        movements = new AbilityMovement[1];
    }
}
