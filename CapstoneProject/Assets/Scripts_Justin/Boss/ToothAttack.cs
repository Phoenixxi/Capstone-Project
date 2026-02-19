using UnityEngine;

/// <summary>
/// Script that controls the boss attack that causes teeth to shoot out of the ground and hurt the player
/// </summary>
public class ToothAttack : BossAttack
{
    //TODO Implement animations for the teeth going down
    [Header("Tooth Attack Settings")]
    [SerializeField] protected Hurtbox toothHurtbox;
    [SerializeField] protected float toothAttackTimer;
    [SerializeField] protected float toothAttackLingerTimer;
    [SerializeField] protected Animator[] molarAnimators;
    [SerializeField] protected Animator toothAnimator;
    [SerializeField] protected Animator uiAnimator;
    protected float currAttackTimer;
    protected float currLingerTimer;

    protected override void Start()
    {
        toothHurtbox.SetElementType(lilGuysNamespace.EntityData.ElementType.Normal);
        toothHurtbox.SetHurtboxDamage(damage);
        uiAnimator = GetComponentInChildren<Animator>();
        base.Start();
    }

    public override void Attack()
    {
        IsAttacking = true;
        currAttackTimer = toothAttackLingerTimer;
        currLingerTimer = toothAttackLingerTimer;
        Debug.Log("Tooth attack starting...");
        foreach (Animator molar in molarAnimators) molar.SetTrigger("Show");
        uiAnimator.SetTrigger("Attacking");
    }

    protected void Update()
    {
        if(currAttackTimer > 0)
        {
            currAttackTimer -= Time.deltaTime;
            if(currAttackTimer <= 0)
            {
                toothAnimator.SetTrigger("Show");
                toothHurtbox.Activate(toothAttackLingerTimer, true);
                Debug.Log("Teeth can damage!");
            }
        } else if(currLingerTimer > 0)
        {
            currLingerTimer -= Time.deltaTime;
            if(currLingerTimer <= 0)
            {
                IsAttacking = false;
                toothAnimator.SetTrigger("Hide");
                foreach (Animator molar in molarAnimators) molar.SetTrigger("Hide");
                Debug.Log("Teeth retracted");
                if (!canRepeat) Destroy(gameObject);
            }
        }
    }
}
