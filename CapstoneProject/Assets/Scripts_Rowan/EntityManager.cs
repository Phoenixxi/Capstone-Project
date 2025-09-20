using UnityEngine;
using lilGuysNamespace;
using UnityEngine.UIElements;

public class EntityManager : MonoBehaviour
{
    [SerializeField] public string entityName;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private SwappingManager swappingManager;
    public float currentHealth;
    public bool isAlive = true;

    [Header("Weapon Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int weaponDamage;
    [SerializeField] private GameObject projectile;

    private Weapon weapon;

    
    void Start()
    {
        // TEMPORARY- change back to maxHealth later
        currentHealth += 20f;
        CreateWeapon();
    }

    private void CreateWeapon()
    {
        //TODO Implement melee weapon generation
        if (projectile != null) weapon = new RangedWeapon(attackCooldown, weaponDamage, projectile);
        else Debug.LogError($"Neither a melee nor ranged weapon could be assigned to {gameObject}. Make sure either the Projectile or Hurtbox fields have a value");
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Entity has died.");
            isAlive = false;
            swappingManager.PlayerHasDied(gameObject);
        }
        else
        {
            Debug.Log("Entity took damage. Current health: " + currentHealth);
        }
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Entity healed. Current health: " + currentHealth);
    }

    /// <summary>
    /// Has the entity attack using their weapon
    /// </summary>
    /// <param name="attackDirection">The direction the attack is facing. If the entity uses a melee wepaon, this parameter is unimportant.</param>
    /// <param name="entityPosition">The attacking entity's position. If the entity uses a melee weapon, this parameter is unimportant.</param>
    public void Attack(Vector3 attackDirection, Vector3 entityPosition)
    {
        if (weapon is RangedWeapon) (weapon as RangedWeapon).UpdateWeaponTransform(attackDirection, entityPosition);
        weapon.Attack();
    }
}
