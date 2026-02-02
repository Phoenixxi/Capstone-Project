using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Class that handles melee attacks
/// </summary>
public class MeleeWeapon : Weapon
{
    private Hurtbox hurtbox;
    private float hurtboxActiveTime;

    public MeleeWeapon(float attackCooldown, int damage, ElementType element, Hurtbox hurtbox, float hurtboxActiveTime, Animator animator) : base(attackCooldown, damage, element, animator)
    {
        this.hurtbox = hurtbox;
        this.hurtboxActiveTime = hurtboxActiveTime;
        hurtbox.SetHurtboxDamage(damage);
        hurtbox.SetElementType(element);
    }

    public override bool Attack()
    {
        if (!HasCooldownExpired()) return false;
        animator.SetTrigger("Shoot");
        return true;
    }

    public override void AttackFromAnimation()
    {
        hurtbox.Activate(hurtboxActiveTime, false);
        lastAttackTime = Time.time;
    }
}
