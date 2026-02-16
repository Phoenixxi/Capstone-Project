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
    protected Animator tentacleAnim;

    protected override void Start()
    {
        tentacleAnim = GetComponentInChildren<Animator>();
        BossAttackEvents events = GetComponentInChildren<BossAttackEvents>();
        events.BecomeDamagingEvent += ActivateHurtbox;
        events.AttackEndedEvent += OnAttackEnded;

        hurtbox.SetHurtboxDamage(damage);
        hurtbox.SetElementType(lilGuysNamespace.EntityData.ElementType.Normal);
        base.Start();
    }

    public override void Attack()
    {
        if(attackType == AttackType.SLAM)
        {
            tentacleAnim.SetTrigger("Slam");
        } else
        {
            tentacleAnim.SetTrigger("Swipe");
        }
    }

    protected void ActivateHurtbox()
    {
        hurtbox.Activate(hurtboxActiveTime, true);
    }

    protected void OnAttackEnded()
    {
        if (!canRepeat) Destroy(gameObject);
    }
}
