using lilGuysNamespace;
using UnityEngine;
using static lilGuysNamespace.EntityData;

public class ExplodeWeapon : Weapon
{
    private ExplodeHurtBox hurtbox;
    public ExplodeWeapon(float attackCooldown, int damage, ElementType element, ExplodeHurtBox hurtbox, Animator animator) : base(attackCooldown, damage, element, animator)
    {
        this.hurtbox = hurtbox;
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
        hurtbox.Activate();
    }
}
