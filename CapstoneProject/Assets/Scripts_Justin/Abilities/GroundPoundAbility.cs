using System;
using UnityEngine;

public class GroundPoundAbility : Ability
{
    [Header("Ground Pound Ability Settings")]
    [SerializeField] private CharacterController entity;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float slamRadius;
    
    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse || entity.isGrounded) return Array.Empty<AbilityMovement>();
        abilityInUse = true;
        movements[0] = new AbilityMovement(Vector3.down * fallSpeed);
        return movements;
    }

    protected override void Update()
    {
        base.Update();
        if(abilityInUse && entity.isGrounded)
        {
            movements[0].Complete();
            abilityInUse = false;
        }
    }


    private void Awake()
    {
        base.Awake();
        movements = new AbilityMovement[1];
    }
}
