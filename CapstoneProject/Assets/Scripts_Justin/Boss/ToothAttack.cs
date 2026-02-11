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
    protected float currAttackTimer;
    protected float currLingerTimer;

    protected override void Start()
    {
        toothHurtbox.SetElementType(lilGuysNamespace.EntityData.ElementType.Normal);
        toothHurtbox.SetHurtboxDamage(damage);
        base.Start();
    }

    public override void Attack()
    {
        currAttackTimer = toothAttackLingerTimer;
        currLingerTimer = toothAttackLingerTimer;
        Debug.Log("Tooth attack starting...");
        //TODO Trigger molar animations
    }

    protected void Update()
    {
        if(currAttackTimer > 0)
        {
            currAttackTimer -= Time.deltaTime;
            if(currAttackTimer <= 0)
            {
                //TODO Activate hurtbox amd play animation
                toothHurtbox.Activate(toothAttackLingerTimer, true);
                Debug.Log("Teeth can damage!");
            }
        } else if(currLingerTimer > 0)
        {
            currLingerTimer -= Time.deltaTime;
            if(currLingerTimer <= 0)
            {
                //TODO Retract teeth
                Debug.Log("Teeth retracted");
                if (!canRepeat) Destroy(gameObject);
            }
        }
    }
}
