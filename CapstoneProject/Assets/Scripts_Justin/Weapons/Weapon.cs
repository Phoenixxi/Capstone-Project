using UnityEngine;

/// <summary>
/// Class that handles attacks for both the player and enemies
/// </summary>
public abstract class Weapon
{
    protected float attackCooldown;
    protected int damage;
    //TODO: Once elements implemented, add element field ehre
    protected float lastAttackTime;

    public Weapon(float attackCooldown, int damage)
    {
        this.attackCooldown = attackCooldown;
        this.damage = damage;
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
