using System;
using UnityEngine;

public class GroundPoundAbility : Ability
{
    [Header("Ground Pound Ability Settings")]
    [SerializeField] private CharacterController entity;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float slamRadius;
    [SerializeField] private float coyoteTime;
    [SerializeField] private LayerMask slamLayerMask;
    private float currentCoyoteTime;


    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse || entity.isGrounded) return Array.Empty<AbilityMovement>();
        abilityInUse = true;
        currentCoyoteTime = 0f;
        movements[0] = new AbilityMovement(Vector3.zero);
        movements[1] = new AbilityMovement(Vector3.down * fallSpeed);
        return movements;
    }

    protected override void Update()
    {
        base.Update();
        if(abilityInUse)
        {
            if(!entity.isGrounded)
            {
                currentCoyoteTime += Time.deltaTime;
                if (currentCoyoteTime >= coyoteTime) movements[0].Complete();
            } else
            {
                movements[1].Complete();
                abilityInUse = false;
                Ray sphereRay = new Ray(transform.position, Vector3.down);
                RaycastHit[] hitEnemies = Physics.SphereCastAll(sphereRay, slamRadius, 0.1f, slamLayerMask);
                foreach (RaycastHit hitEntity in hitEnemies)
                {
                    Debug.Log($"Hit {hitEntity.transform.gameObject}", hitEntity.transform.gameObject);
                    EntityManager enemy = hitEntity.transform.GetComponent<EntityManager>();
                    if (enemy == null) continue;
                    enemy.TakeDamage(damage, element);
                }
                currentCooldown = cooldown;
            }
        }
    }


    protected void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
        Gizmos.DrawSphere(transform.position, slamRadius);
    }

    protected override void Awake()
    {
        base.Awake();
        movements = new AbilityMovement[2];
    }
}
