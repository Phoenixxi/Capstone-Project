using System;
using UnityEngine;

public class GroundPoundAbility : Ability
{
    [Header("Ground Pound Ability Settings")]
    [SerializeField] private CharacterController entity;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float slamRadius;
    [SerializeField] private LayerMask slamLayerMask;
    
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
            Ray sphereRay = new Ray(transform.position, Vector3.down);
            RaycastHit[] hitEnemies = Physics.SphereCastAll(sphereRay, slamRadius, 0.1f, slamLayerMask);
            foreach(RaycastHit hitEntity in hitEnemies)
            {
                Debug.Log($"Hit {hitEntity.transform.gameObject}", hitEntity.transform.gameObject);
                EntityManager enemy = hitEntity.transform.GetComponent<EntityManager>();
                if (enemy == null) continue;
                enemy.TakeDamage(damage, element);
            }
        }
    }


    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
        Gizmos.DrawSphere(transform.position, slamRadius);
    }

    private void Awake()
    {
        base.Awake();
        movements = new AbilityMovement[1];
    }
}
