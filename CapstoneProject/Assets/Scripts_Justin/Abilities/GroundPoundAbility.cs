using System;
using UnityEngine;

/// <summary>
/// Ground pound ability that slams the entity down and damages enemies in a distance around the impact area
/// </summary>
public class GroundPoundAbility : Ability
{
    [Header("Ground Pound Ability Settings")]
    [SerializeField] private CharacterController entity;
    [SerializeField] private float fallSpeed;
    [SerializeField] private float slamRadius;
    [SerializeField] private float coyoteTime;
    [SerializeField] private float landingFreezeTime;
    [SerializeField] private LayerMask slamLayerMask;
    [SerializeField] private float knockbackDelayTime;
    [SerializeField] private float knockbackStrength;
    [SerializeField] private float knockbackDuration;
    [SerializeField] private GameObject boomVFXPrefab;
    [SerializeField] public Animator animator;
    private float currentCoyoteTime;
    private float currentLandingFreezeTime;

    //VFX
    private GameObject vfxInstance;


    public override AbilityMovement[] UseAbility(Vector2 horizontalDirection)
    {
        if (currentCooldown > 0f || abilityInUse || entity.isGrounded) return Array.Empty<AbilityMovement>();
        animator.SetTrigger("GroundPound");
        abilityInUse = true;
        currentCoyoteTime = 0f;
        currentLandingFreezeTime = 0f;
        movements[0] = new AbilityMovement(Vector3.zero);
        movements[1] = new AbilityMovement(Vector3.down * fallSpeed);
        movements[2] = new AbilityMovement(Vector3.down);
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
            } else if (!movements[1].HasEnded())
            {
                animator.SetTrigger("Grounded");
                //VFX
                vfxInstance = Instantiate(boomVFXPrefab, transform.position, Quaternion.identity);

                movements[1].Complete();
                //abilityInUse = false;
                Ray sphereRay = new Ray(transform.position, Vector3.down);
                RaycastHit[] hitEnemies = Physics.SphereCastAll(sphereRay, slamRadius, 0.1f, slamLayerMask);
                foreach (RaycastHit hitEntity in hitEnemies)
                {
                    Debug.Log($"Hit {hitEntity.transform.gameObject}", hitEntity.transform.gameObject);
                    EntityManager enemy = hitEntity.transform.GetComponent<EntityManager>();
                    Vector3 knockback = (enemy.transform.position - transform.position).normalized;
                    knockback *= knockbackStrength;
                    KnockbackMovement knockbackDelay = new KnockbackMovement(Vector3.zero, Time.time, knockbackDelayTime);
                    KnockbackMovement knockbackMov = new KnockbackMovement(knockback, Time.time + knockbackDelayTime, knockbackDuration);
                    if (enemy == null) continue;
                    enemy.TakeDamage(damage, element, new KnockbackMovement[] { knockbackDelay, knockbackMov});
                }
                cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
            } else
            {
                currentLandingFreezeTime += Time.deltaTime;
                if(currentLandingFreezeTime >= landingFreezeTime)
                {
                    currentCooldown = cooldown;
                    abilityInUse = false;
                    movements[2].Complete();
                }
            }
        }
    }


    //protected void OnDrawGizmos()
    //{
    //    Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
    //    Gizmos.DrawSphere(transform.position, slamRadius);
    //}

    protected override void Awake()
    {
        base.Awake();
        if(boomVFXPrefab == null)
            Debug.LogError("Boom VFX Prefab is not assigned in the inspector for Boom > GroundPoundAbility");
        movements = new AbilityMovement[3];
    }

    public override void Cancel()
    {
        if (!abilityInUse) return;
        movements[0].Complete();
        movements[1].Complete();
        movements[2].Complete();
        animator.SetTrigger("Grounded");
        currentCooldown = cooldown;
        abilityInUse = false;
    }
}
