using UnityEngine;

/// <summary>
/// Class that handles attacks for both the player and enemies
/// </summary>
public abstract class Weapon
{
    private float attackCooldown;
    private int damage;
    //TODO: Once elements implemented, add element field ehre
    private float lastAttackTime;

    public Weapon(float attackCooldown, int damage)
    {
        this.attackCooldown = attackCooldown;
        this.damage = damage;
        lastAttackTime = 0f;
    }

    /// <summary>
    /// Executes an attack if called after the attack cooldown has expried
    /// </summary>
    public void Attack()
    {
        float currentAttackTime = Time.time;
        if (currentAttackTime - lastAttackTime < attackCooldown) return;
    }
}
