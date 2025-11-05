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
    protected float baseCooldown;

    public Weapon(float attackCooldown, int damage, ElementType element)
    {
        this.attackCooldown = attackCooldown;
        this.damage = damage;
        this.element = element;
        baseCooldown = attackCooldown;
        lastAttackTime = 0f;
    }

    /// <summary>
    /// Executes an attack if called after the attack cooldown has expried
    /// </summary>
    public abstract bool Attack();

    /// <summary>
    /// Checks whether the weapon's attack cooldown has finished
    /// </summary>
    /// <returns></returns>
    protected bool HasCooldownExpired()
    {
        float currentAttackTime = Time.time;
        return currentAttackTime - lastAttackTime >= attackCooldown;
    }

    /// <summary>
    /// Multiplies this weapon's attack cooldown by the given amount
    /// </summary>
    /// <param name="multiplier">The amount to multiply the attack cooldown by</param>
    public void ApplyCooldownMultiplier(float multiplier)
    {
        attackCooldown *= multiplier;
    }

    /// <summary>
    /// Resets the weapon's fire rate back to its base value
    /// </summary>
    public void RestoreBaseFireRate()
    {
        attackCooldown = baseCooldown;
    }
}
