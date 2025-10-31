using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using ElementType = lilGuysNamespace.EntityData.ElementType;
using UnityEngine.AI;
using System;

public class EntityManager : MonoBehaviour
{
    [SerializeField] public string entityName;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public ElementType defaultElement;  // Will never change
    [SerializeField] public ElementType taggedElement = ElementType.Normal;   // default and tagged will be the same if default != Normal
    [SerializeField] private SwappingManager swappingManager;
    [SerializeField] private CharacterController entityMovement;
    public float currentHealth;
    public bool isAlive = true;
    public AbilityData data;
    private bool usesNavAgent;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private int extraJumps = 0;
    private int currentExtraJumps;

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

    // DESIGNERS: Adjust fields here
    private float dmgMultiplier = 2.0f;

    // VFX info
    private GameObject currentVFXInstance;
    private Transform vfxAnchor;

    public Action<float, float, ElementType> OnHealthUpdatedEvent;

    
    void Start()
    {
        // TEMPORARY- change back to maxHealth later
        currentHealth = maxHealth;
        //if (gameObject.CompareTag("Enemy"))
        //    currentHealth = 1;
        CreateWeapon();
        ability = GetComponent<Ability>();
        movementQueue = new Queue<AbilityMovement>();

        // Set tagged element to default if default != normal
        if (defaultElement != ElementType.Normal) 
            taggedElement = defaultElement;
        if (GetComponent<NavMeshAgent>() != null) usesNavAgent = true;
    }

    void Awake()
    {
        if (gameObject.CompareTag("Enemy"))
        {
            vfxAnchor = transform.Find("VFXanchor");
        }

    }

    private void CreateWeapon()
    {
        //TODO Replace default element with the entity-specific one
        if (projectile != null) weapon = new RangedWeapon(attackCooldown, weaponDamage, defaultElement, projectile);
        else if (meleeHurtbox != null) weapon = new MeleeWeapon(attackCooldown, weaponDamage, defaultElement, meleeHurtbox, hurtboxActivationTime);
        else Debug.LogError($"Neither a melee nor ranged weapon could be assigned to {gameObject}. Make sure either the Projectile or Hurtbox fields have a value");
    }

    /// <summary>
    /// Makes the entity's move based off normal movement conditions (regular horizontal movement, jumping, etc.)
    /// </summary>
    private void HandleDefaultMovement()
    {
        if (usesNavAgent) return;
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
        if (entityMovement.isGrounded) currentExtraJumps = extraJumps;
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
        //if (!entityMovement.isGrounded) return;
        if(!entityMovement.isGrounded)
        {
            if (currentExtraJumps == 0) return;
            currentExtraJumps--;
        }
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

    private void ApplyVFX(ElementType element)
    {
        ClearVFX();
        if (element == ElementType.Zoom)
        {
            currentVFXInstance = Instantiate(Resources.Load<GameObject>("VFX_ElemAffected_ZoomTEMP"), vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }
        else if (element == ElementType.Boom)
        {
            currentVFXInstance = Instantiate(Resources.Load<GameObject>("VFX_ElemAffected_BoomTEMP"), vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }
        else if (element == ElementType.Gloom)
        {
            currentVFXInstance = Instantiate(Resources.Load<GameObject>("VFX_ElemAffected_GloomTEMP"), vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }

    }

    private void ClearVFX()
    {
        if (currentVFXInstance != null)
        {
            Destroy(currentVFXInstance);
            currentVFXInstance = null;
        }
    }


    // TODO: Get rid of this method instance (referenced in PlayerController.TakeDamageWrapper())
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
            //swappingManager.PlayerHasDied(gameObject);
        }
        else
        {
            Debug.Log("Entity took damage. Current health: " + currentHealth);
        }
    }

    /// <summary>
    /// Called when the entity is attacked (with either a weapon or an ability)
    /// </summary>
    /// <param name="damage">The amount of damage being delt.</param>
    /// <param name="element">The incoming element.</param>
    public void TakeDamage(float damage, ElementType element)
    {
        float newHealth = currentHealth - damage;
        if (newHealth <= 0)
        {
            EntityHasDied();
            return;
        }
        else
        {
             //If entity is not tagged or if they are hit with their same element
            if (taggedElement == ElementType.Normal || taggedElement == element)  
            {
                taggedElement = element;
                currentHealth = newHealth;
            }
            else    // entity is already tagged and they were hit with different element, start a reaction
            {
                Reaction(element, damage);
            }
        }
        if (OnHealthUpdatedEvent != null) OnHealthUpdatedEvent(currentHealth, maxHealth, taggedElement);
    }

    /// <summary>
    /// Called when the entity's health has reached 0 or less
    /// </summary>
    private void EntityHasDied()
    {
        currentHealth = 0;
        Debug.Log("Entity has died.");
        isAlive = false;

        if(this.gameObject.CompareTag("Enemy"))
        {
            SpawnHealthPack spawnPack = GetComponent<SpawnHealthPack>();
            spawnPack.Spawn(gameObject.transform.position);
        }


        if (this.gameObject.CompareTag("Player"))
        {
            swappingManager.PlayerHasDied(gameObject);
        }
        else Destroy(gameObject); // I dont think we want to destroy the player.....
    }


    private void DropHealthPack()
    {

    }

    /// <summary>
    /// Signaled when a reaction occurs between two different elements
    /// </summary>
    /// <param name="initiatingElement">The second element that caused the reaction.</param>
    /// <param name="incomingDmg">The amount of damage being delt to the enemy before reaction.</param>
    private void Reaction( ElementType initiatingElement, float incomingDmg)
    {  
        // ZOOM x BOOM
        if((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Boom || initiatingElement == ElementType.Boom))
        {
            float newHealth = currentHealth - (incomingDmg * dmgMultiplier);
            Debug.Log("currentHealth: " + currentHealth + " incomingDmg: " + incomingDmg + " incomingxdmgMult: " + (incomingDmg * dmgMultiplier));
            if(newHealth <= 0)
            {
                EntityHasDied();
                return;
            }
            // If entity survived, set new health and reset their tagged element to default
            currentHealth = newHealth;
            taggedElement = defaultElement;
        }
        // ZOOM x GLOOM
        else if((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Gloom || initiatingElement == ElementType.Gloom))
        {
             Debug.Log("in zoomxgloom");
            currentHealth -= incomingDmg;
            taggedElement = defaultElement; // Reset tag to default/starting element

            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                Debug.Log("ApplySlow called");
                effectable.ApplySlow(data);
            }
        }
        // BOOM x GLOOM
        else
        {
            currentHealth -= incomingDmg;
            taggedElement = defaultElement; // Reset tag to default/starting element

            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                effectable.ApplyEffect(data);
            }
        }

        

    } 

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Entity healed. Current health: " + currentHealth);
        if(OnHealthUpdatedEvent != null) OnHealthUpdatedEvent(currentHealth, maxHealth, taggedElement);
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
