using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;

public class EntityManager : MonoBehaviour
{
    [SerializeField] public string entityName;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] private SwappingManager swappingManager;
    [SerializeField] private CharacterController entityMovement;
    public float currentHealth;
    public bool isAlive = true;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = 10f;

    [Header("Universal Weapon Settings")]
    [SerializeField] private float attackCooldown;
    [SerializeField] private int weaponDamage;

    [Header("Ranged Weapon Settings: Keep Empty If Not Using")]
    [SerializeField] private GameObject projectile;

    [Header("Melee Weapon Settings: Keep Empty If Not Using")]
    [SerializeField] private Hurtbox meleeHurtbox;
    [SerializeField] private float hurtboxActivationTime;

    [Header("Ability Settings Must Be Changed In Ability Script")]

    private Weapon weapon;
    private Vector3 movementVelocity;
    private Ability ability;
    //Keeps track of movements associated with abilities; if this queue is not empty, these movements must be performed before standard movements can
    private Queue<AbilityMovement> movementQueue;

    
    void Start()
    {
        // TEMPORARY- change back to maxHealth later
        currentHealth += 20f;
        CreateWeapon();
        ability = GetComponent<Ability>();
        movementQueue = new Queue<AbilityMovement>();
    }

    private void CreateWeapon()
    {
        //TODO Replace default element with the entity-specific one
        if (projectile != null) weapon = new RangedWeapon(attackCooldown, weaponDamage, EntityData.ElementType.Normal, projectile);
        else if (meleeHurtbox != null) weapon = new MeleeWeapon(attackCooldown, weaponDamage, EntityData.ElementType.Normal, meleeHurtbox, hurtboxActivationTime);
        else Debug.LogError($"Neither a melee nor ranged weapon could be assigned to {gameObject}. Make sure either the Projectile or Hurtbox fields have a value");
    }

    /// <summary>
    /// Makes the entity's move based off normal movement conditions (regular horizontal movement, jumping, etc.)
    /// </summary>
    private void HandleDefaultMovement()
    {
        if (entityMovement.isGrounded && movementVelocity.y < 0) movementVelocity.y = -2f;
        movementVelocity.y -= gravity * Time.deltaTime;
        entityMovement.Move(movementVelocity * Time.deltaTime);
    }

    /// <summary>
    /// Makes the entity move based on the first movement in the movement queue. If that first move is finished, it is cleared
    /// </summary>
    private void HandleQueueMovement()
    {
        AbilityMovement currentMovement = movementQueue.Peek();
        if (currentMovement.HasEnded())
        {
            movementQueue.Dequeue();
            if (movementQueue.Count == 0) movementVelocity.y = 0f;
        }
        else entityMovement.Move(currentMovement.GetMovementVelocity() * Time.deltaTime);
    }

    void Update()
    {
        if (movementQueue.Count > 0) HandleQueueMovement();
        else HandleDefaultMovement();
    }

    /// <summary>
    /// Updates the entity's X and Z movement based off a given input vector. The input vector is expected to be normalized and only handles horizontal movement
    /// </summary>
    /// <param name="input">The direction the entity should be moving</param>
    public void SetInputDirection(Vector2 input)
    {
        movementVelocity.x = input.x * movementSpeed;
        movementVelocity.z = input.y * movementSpeed;
    }

    /// <summary>
    /// Updates the entity's movement velocity. This vector can include both horizontal and vertical movement and is not expected to be normalized
    /// </summary>
    /// <param name="newVelocity">The new velocity the entity should move</param>
    public void SetMovementVelocity(Vector3 newVelocity)
    {
        movementVelocity = newVelocity;
    }

    /// <summary>
    /// Returns the entity's current movement velocity
    /// </summary>
    /// <returns>A Vector3 representing the entity's movement</returns>
    public Vector3 GetMovementVelocity()
    {
        return movementVelocity;
    }

    /// <summary>
    /// Makes the entity jump if they are grounded
    /// </summary>
    public void Jump()
    {
        if (!entityMovement.isGrounded) return;
        movementVelocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
    }

    /// <summary>
    /// Attempts to use this entity's ability
    /// </summary>
    /// <param name="horizontalDirection">The direction the entity is moving horizontally at the time of activation</param>
    public void UseAbility(Vector2 horizontalDirection)
    {
        if(ability != null)
        {
            AbilityMovement[] movementList = ability.UseAbility(horizontalDirection);
            foreach (AbilityMovement movement in movementList) movementQueue.Enqueue(movement);
        } else
        {
            Debug.LogError("No ability assigned; make sure an ability script has been attatched to this game object");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Entity has died.");
            isAlive = false;
            Destroy(gameObject);
            return;
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
    /// Has the entity attack using their weapon if they aren't currently using an ability
    /// </summary>
    /// <param name="attackDirection">The direction the attack is facing. If the entity uses a melee wepaon, this parameter is unimportant.</param>
    /// <param name="entityPosition">The attacking entity's position. If the entity uses a melee weapon, this parameter is unimportant.</param>
    public void Attack(Vector3 attackDirection, Vector3 entityPosition)
    {
        if (AbilityInUse()) return;
        if (weapon is RangedWeapon) (weapon as RangedWeapon).UpdateWeaponTransform(attackDirection, entityPosition);
        weapon.Attack();
    }

    /// <summary>
    /// Returns whether or not this entity's ability is being used
    /// </summary>
    /// <returns></returns>
    public bool AbilityInUse()
    {
        if (ability == null) return false;
        return ability.AbilityInUse();
    }

    
}
