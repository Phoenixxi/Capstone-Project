using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Class that handles melee attacks
/// </summary>
public class MeleeWeapon : Weapon
{
    private Hurtbox hurtbox;
    private float hurtboxActiveTime;

    public MeleeWeapon(float attackCooldown, int damage, ElementType element, Hurtbox hurtbox, float hurtboxActiveTime) : base(attackCooldown, damage, element)
    {
        this.hurtbox = hurtbox;
        this.hurtboxActiveTime = hurtboxActiveTime;
        hurtbox.SetHurtboxDamage(damage);
    }

    public override bool Attack()
    {
        if (!HasCooldownExpired()) return false;
        hurtbox.Activate(hurtboxActiveTime);
        lastAttackTime = Time.time;
        return true;
    }
}
