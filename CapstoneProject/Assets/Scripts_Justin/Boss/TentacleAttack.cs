using UnityEngine;

/// <summary>
/// Class that controls the boss's tentacle attack. Can be set to perform either a slam or a swipe.
/// </summary>
public class TentacleAttack : BossAttack
{
    protected enum AttackType
    {
        SLAM,
        SWIPE
    }
    
    [Header("Tentacle Attack Settings")]
    [SerializeField] protected Hurtbox hurtbox;
    [SerializeField] protected float hurtboxActiveTime;
    [SerializeField] protected AttackType attackType;
    [SerializeField] protected bool followsPlayer = false;
    [SerializeField] protected float rotationSpeed = 1f;
    protected Animator tentacleAnim;
    protected bool isFollowingPlayer;
    protected Transform playerTransform;

    protected override void Start()
    {
        isFollowingPlayer = false;
        playerTransform = FindFirstObjectByType<PlayerController>().transform;
        tentacleAnim = GetComponentInChildren<Animator>();
        BossAttackEvents events = GetComponentInChildren<BossAttackEvents>();
        events.BecomeDamagingEvent += ActivateHurtbox;
        events.AttackEndedEvent += OnAttackEnded;
        hurtbox.SetHurtboxDamage(damage);
        hurtbox.SetElementType(lilGuysNamespace.EntityData.ElementType.Normal);
        tentacleAnim.SetFloat("SpeedMultiplier", speedupAmount);
        base.Start();
    }

    public override void Attack()
    {
        IsAttacking = true;
        if(attackType == AttackType.SLAM)
        {
            tentacleAnim.SetTrigger("Slam");
        } else
        {
            tentacleAnim.SetTrigger("Swipe");
        }
        if (followsPlayer) isFollowingPlayer = true;
    }

    protected void Update()
    {
        if (!isFollowingPlayer) return;
        Quaternion targetRotation = Quaternion.LookRotation(-(playerTransform.position - transform.position), Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    protected void ActivateHurtbox()
    {
        isFollowingPlayer = false;
        hurtbox.Activate(hurtboxActiveTime, true);
    }

    protected void OnAttackEnded()
    {
        IsAttacking = false;
        if (!canRepeat) Destroy(gameObject);
    }

    public override void ApplySpeedup()
    {
        tentacleAnim.SetBool("IsSpedUp", true);
    }
}
