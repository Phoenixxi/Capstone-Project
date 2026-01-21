using TMPro;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Class that handles ranged attacks that utilize projectiles
/// </summary>
public class RangedWeapon : Weapon
{
    private GameObject projectile;
    private Projectile projectileScript;
    private Vector3 attackDirection;
    private Vector3 currentLocation;
    private bool hasLifesteal;
    private float lifeStealPercentage;
    private PlayerController player;

    public RangedWeapon(float attackCooldown, int damage, ElementType element, GameObject projectile, bool hasLifesteal = false, float lifeStealPercentage = 1) : base(attackCooldown, damage, element)
    {
        this.projectile = projectile;
        attackDirection = Vector3.zero;
        this.hasLifesteal = hasLifesteal;
        if (hasLifesteal) player = Object.FindFirstObjectByType<PlayerController>();
        this.lifeStealPercentage = lifeStealPercentage;
    }

    public override bool Attack()
    {
        if (!HasCooldownExpired()) return false;
        Vector3 projectileSpawnOffset = attackDirection * 0f;
        Vector3 projectileSpawnPosition = currentLocation + projectileSpawnOffset;
        projectileScript = GameObject.Instantiate(projectile, projectileSpawnPosition, Quaternion.identity).GetComponent<Projectile>();

        if (projectileScript == null)
        {
            Debug.LogError($"Projectile {projectile} is missing Projectile Monobehavior Script. Make sure the GameObject being used has a Projectile script attatched to it.");
            return false;
        }

        projectileScript.SetProjectileDamage(damage);
        projectileScript.SetProjectileElement(element);
        projectileScript.ChangeMoveDirection(attackDirection);
        lastAttackTime = Time.time;
        if (hasLifesteal) projectileScript.OnProjectileHitEntity += GiveLifesteal;
        return true;
        //Debug.Log("Projectile fired!");
    }

    /// <summary>
    /// Updates the ranged weapon's current attack direction and the position of the entity that its attatched to. This function should be run
    /// before calling Attack() if either of these properties are expected to change over time
    /// </summary>
    /// <param name="attackDirection">The direction the weapon's projectile will be fired in</param>
    /// <param name="entityPosition">The current position of the weapon's "user"</param>
    public void UpdateWeaponTransform(Vector3 attackDirection, Vector3 entityPosition)
    {
        this.attackDirection = attackDirection;
        this.currentLocation = entityPosition;
    }

    /// <summary>
    /// Provides the player (specifically) with lifesteal if the weapon has it enabled
    /// </summary>
    /// <param name="damage"></param>
    private void GiveLifesteal(int damage)
    {
        player.HealAllCharacters((float)damage * lifeStealPercentage);
    }
}
