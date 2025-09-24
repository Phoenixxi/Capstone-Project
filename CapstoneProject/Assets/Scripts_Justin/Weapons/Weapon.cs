using lilGuysNamespace;
using UnityEngine;

using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Class that handles attacks for both the player and enemies
/// </summary>
public abstract class Weapon
{
    protected float attackCooldown;
    protected int damage;
    protected ElementType element;
    protected float lastAttackTime;

    public Weapon(float attackCooldown, int damage, ElementType element)
    {
        this.attackCooldown = attackCooldown;
        this.damage = damage;
        this.element = element;
        lastAttackTime = 0f;
    }

    /// <summary>
    /// Executes an attack if called after the attack cooldown has expried
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Checks whether the weapon's attack cooldown has finished
    /// </summary>
    /// <returns></returns>
    protected bool hasCooldownExpired()
    {
        float currentAttackTime = Time.time;
        return currentAttackTime - lastAttackTime >= attackCooldown;
    }
}
