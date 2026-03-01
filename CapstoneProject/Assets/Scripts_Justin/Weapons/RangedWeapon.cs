using System;
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

    private PlayerController player;

    private int projectileCount;
    private float perBulletSpread; //Treated as degrees


    public RangedWeapon(float attackCooldown, int damage, ElementType element, GameObject projectile, Animator animator,
        int projectileCount = 1, float perBulletSpread = 0f) : base(attackCooldown, damage, element, animator)
    {
        this.projectile = projectile;
        attackDirection = Vector3.zero;
        this.projectileCount = projectileCount;
        this.perBulletSpread = perBulletSpread;
    }

    public override bool Attack()
    {
        if (!HasCooldownExpired()) return false;
        animator.SetTrigger("Shoot");
        //Vector3 projectileSpawnOffset = attackDirection * 0f;
        //Vector3 projectileSpawnPosition = currentLocation + projectileSpawnOffset;
        //projectileScript = GameObject.Instantiate(projectile, projectileSpawnPosition, Quaternion.identity).GetComponent<Projectile>();

        //if (projectileScript == null)
        //{
        //    Debug.LogError($"Projectile {projectile} is missing Projectile Monobehavior Script. Make sure the GameObject being used has a Projectile script attatched to it.");
        //    return false;
        //}

        //projectileScript.SetProjectileDamage(damage);
        //projectileScript.SetProjectileElement(element);
        //projectileScript.ChangeMoveDirection(attackDirection);
        //if (hasLifesteal) projectileScript.OnProjectileHitEntity += GiveLifesteal;
        return true;
        //Debug.Log("Projectile fired!");
    }

    /// <summary>
    /// Spawns a projectile and assigns it with the appropriate characteristics.
    /// </summary>
    /// <param name="rotation">The angle the projectile spawns at relative to the aiming direction. Leave empty if only firing in a straight line</param>
    private void SpawnProjectile(Quaternion? rotation = null)
    {
        Vector3 adjustedAttackDirection = (rotation != null) ? (Quaternion)rotation * attackDirection : attackDirection;
        Vector3 projectileSpawnOffset = adjustedAttackDirection * 0f;
        Vector3 projectileSpawnPosition = currentLocation + projectileSpawnOffset;
        projectileScript = GameObject.Instantiate(projectile, projectileSpawnPosition, Quaternion.identity).GetComponent<Projectile>();

        if (projectileScript == null)
        {
            Debug.LogError($"Projectile {projectile} is missing Projectile Monobehavior Script. Make sure the GameObject being used has a Projectile script attatched to it.");
            return;
        }

        projectileScript.SetProjectileDamage(damage);
        projectileScript.SetProjectileElement(element);
        //projectileScript.ChangeMoveDirection(attackDirection);
        projectileScript.ChangeMoveDirection(adjustedAttackDirection);
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


    public override void AttackFromAnimation()
    {
        Debug.Log("Attacking from animation");
        int currentProjectileNum = 0;
        if(projectileCount % 2 != 0)
        {
            SpawnProjectile();
            currentProjectileNum++;
        }
        float currentAngle = perBulletSpread;
        while(currentProjectileNum < projectileCount)
        {
            SpawnProjectile(Quaternion.AngleAxis(currentAngle, Vector3.up));
            SpawnProjectile(Quaternion.AngleAxis(-currentAngle, Vector3.up));
            currentProjectileNum += 2;
        }

        lastAttackTime = Time.time;
    }
}
