using System;
using UnityEngine;

/// <summary>
/// Behavior for dash ability that damages enemies that pass through it.
/// </summary>
public class DashAbility : Ability
{
    [Header("Dash Ability Settings")]
    [SerializeField] private float distance;
    [SerializeField] private float speed;
    [SerializeField] private Hurtbox dashHurtbox;
    [SerializeField] private Collider entityCollision;
    [SerializeField] private LayerMask dashPhasingLayers;
    [SerializeField] private GameObject zoomVFXPrefab;
    [SerializeField] public Animator animator;

    private float currentDashTimer;
    private float dashTimer;

    private float duration = 1f;
    private GameObject vfxInstance;

    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse) return Array.Empty<AbilityMovement>();
        animator.SetTrigger("Dash");
        // VFX
        vfxInstance = Instantiate(zoomVFXPrefab, transform.position, Quaternion.identity);

        abilityInUse = true;
        dashHurtbox.Activate(dashTimer);
        Vector3 movementVector;
        if (horizontalDirection == Vector2.zero) movementVector = Vector3.right * speed; 
        else movementVector = new Vector3(horizontalDirection.x, 0f, horizontalDirection.y) * speed;
        movements[0] = new AbilityMovement(movementVector);
        entityCollision.excludeLayers += dashPhasingLayers;
        return movements;
    }

    protected override void Awake()
    {
        base.Awake();
        if(zoomVFXPrefab == null)
            Debug.LogError("Zoom VFX Prefab is not assigned in the inspector for Zoom > DashAbility");
        
        movements = new AbilityMovement[1];
        currentDashTimer = 0f;
        dashTimer = distance / speed;
        dashHurtbox.SetHurtboxDamage(damage);
    }

    protected override void Update()
    {
        base.Update();
        if (abilityInUse) currentDashTimer += Time.deltaTime;
        if(currentDashTimer >= dashTimer)
        {
            animator.SetTrigger("NotDash");
            currentDashTimer = 0f;
            movements[0].Complete();
            abilityInUse = false;
            entityCollision.excludeLayers -= dashPhasingLayers;
            currentCooldown = cooldown;
        }
    }

}
