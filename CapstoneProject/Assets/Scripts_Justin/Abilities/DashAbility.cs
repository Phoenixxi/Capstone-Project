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
    [SerializeField] private GameObject zoomVFXPrefabR;
    [SerializeField] private GameObject zoomVFXPrefabL;
    [SerializeField] public Animator animator;

    private float currentDashTimer;
    private float dashTimer;
    [SerializeField] private GameObject playerVFXAnchor;
    
    private GameObject vfxInstance;
    private Transform vfxAnchor;

    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse) return Array.Empty<AbilityMovement>();
        animator.SetTrigger("Dash");
        // VFX
        if(horizontalDirection.x < 0f)
            vfxInstance = Instantiate(zoomVFXPrefabL, vfxAnchor.position, Quaternion.identity, playerVFXAnchor.transform);
        else
            vfxInstance = Instantiate(zoomVFXPrefabR, vfxAnchor.position, Quaternion.identity, playerVFXAnchor.transform);
        

        abilityInUse = true;
        dashHurtbox.Activate(dashTimer, true);
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
        if(zoomVFXPrefabR == null || zoomVFXPrefabL == null)
            Debug.LogError("Zoom VFX Dash is not assigned in the inspector for Zoom > DashAbility");
        
        movements = new AbilityMovement[1];
        vfxAnchor = transform.Find("VFXanchor");
        currentDashTimer = 0f;
        dashTimer = distance / speed;
        dashHurtbox.SetHurtboxDamage(damage);
        dashHurtbox.SetElementType(lilGuysNamespace.EntityData.ElementType.Zoom);
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

    public override void Cancel()
    {
        if (!abilityInUse) return;
        animator.SetTrigger("NotDash");
        currentDashTimer = 0f;
        movements[0].Complete();
        abilityInUse = false;
        entityCollision.excludeLayers -= dashPhasingLayers;
        currentCooldown = cooldown;
    }
}
