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

    [Header("Animation")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

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

    [SerializeField] private GameObject damageNumberVFXPrefab;
    [SerializeField] private float damageNumberDisplayTime;

    // VFX info
    [Header("Elemental glow VFX: Needed for both player and enemy")]
    [SerializeField] private GameObject zoomElementVFX;
    [SerializeField] private GameObject boomElementVFX;
    [SerializeField] private GameObject gloomElementVFX;

    [Header("Player Attack VFX")]
    [SerializeField] private GameObject zoomAttackVFX;


    [Header("Boom Only")]
    [SerializeField] private GameObject boomJumpVFX;    

    [Header("Elemental Reactions: ENEMY ONLY")]
    [SerializeField] public GameObject boomGloomReactionVFX;
    [SerializeField] private GameObject boomZoomReactionVFX;
    [SerializeField] private GameObject gloomZoomReactionVFX;


    [Header("On Hit Effect: ENEMY ONLY")]
    [SerializeField] private GameObject zoomHitVFX;
    [SerializeField] private GameObject boomHitVFX;
    [SerializeField] private GameObject gloomHitVFX;

    [Header("Enemy Death: ENEMY ONLY")]
    [SerializeField] private GameObject enemyDeathVFX;

    private GameObject currentElementalVFXInstance;
    private GameObject currentZoomAttackVFX;
    private GameObject currentHitAttackVFX;
    private GameObject currentEnemyDeathVFX;
    private Transform vfxAnchor;
    private Transform vfxHitAnchor;

    public Action<float, float, ElementType> OnHealthUpdatedEvent;

    //Events designed for making playing sounds easier
    public Action OnEntityHurtEvent;
    public Action OnJumpEvent;
    public Action OnEntityKilledEvent;



    // Initialization =======================================================================================================================   
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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
        vfxAnchor = transform.Find("VFXanchor");
        if(gameObject.CompareTag("Enemy"))
            vfxHitAnchor = transform.Find("VFXHitAnchor");
    }

    void Update()
    {
        if (movementQueue.Count > 0) HandleQueueMovement();
        else HandleDefaultMovement();
        if (entityMovement.isGrounded) currentExtraJumps = extraJumps;
    }

    public void SetHealthToFull()
    {
        currentHealth = maxHealth;
    }

    private void CreateWeapon()
    {
        //TODO Replace default element with the entity-specific one
        if (projectile != null) weapon = new RangedWeapon(attackCooldown, weaponDamage, defaultElement, projectile);
        else if (meleeHurtbox != null) weapon = new MeleeWeapon(attackCooldown, weaponDamage, defaultElement, meleeHurtbox, hurtboxActivationTime);
        else Debug.LogError($"Neither a melee nor ranged weapon could be assigned to {gameObject}. Make sure either the Projectile or Hurtbox fields have a value");
    }




    // Movement =============================================================================================================================
    /// <summary>
    /// Makes the entity's move based off normal movement conditions (regular horizontal movement, jumping, etc.)
    /// </summary>
    private void HandleDefaultMovement()
    {
        if (usesNavAgent) return;
        if (entityMovement.isGrounded && movementVelocity.y < 0) 
            movementVelocity.y = -2f;
        movementVelocity.y -= gravity * Time.deltaTime;
        animator.SetFloat("Speed", movementVelocity.magnitude);
        animator.SetFloat("JumpVelocity", movementVelocity.y);
        animator.SetBool("isJumpingUp", movementVelocity.y > 0);
        animator.SetBool("isJumpingDown", movementVelocity.y < -2.1f);
        spriteRenderer.flipX = movementVelocity.x < 0f;
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

    /// <summary>
    /// Returns the entity's current movement velocity
    /// </summary>
    /// <returns>A Vector3 representing the entity's movement</returns>
    public Vector3 GetMovementVelocity()
    {
        return movementVelocity;
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
    /// Updates the entity's X and Z movement based off a given input vector. The input vector is expected to be normalized and only handles horizontal movement
    /// </summary>
    /// <param name="input">The direction the entity should be moving</param>
    public void SetInputDirection(Vector2 input)
    {
        movementVelocity.x = input.x * movementSpeed;
        movementVelocity.z = input.y * movementSpeed;
    }

    /// <summary>
    /// Makes the entity jump if they are grounded
    /// </summary>
    public void Jump()
    {
        //if (!entityMovement.isGrounded) return;
        if(!entityMovement.isGrounded)
        {
            if (currentExtraJumps == 0) 
                return;
            Instantiate(boomJumpVFX, vfxAnchor.position, Quaternion.identity);
            currentExtraJumps--;
        }
        movementVelocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
        OnJumpEvent?.Invoke();
    }



    // Abilities / Damage / Attacking ====================================================================================================================
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
        ShowDamageNumber((int)damage, ElementType.Normal);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EntityHasDied();
            isAlive = false;
            return;
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
            ShowDamageNumber((int)damage, element);
            EntityHasDied();
            isAlive = false;
            return;
        }
        else if (element != ElementType.Normal)
        {
            //If entity is not tagged or if they are hit with their same element
            if (taggedElement == ElementType.Normal || taggedElement == element)
            {
                taggedElement = element;
                currentHealth = newHealth;
                ShowDamageNumber((int)damage, element);
                if (gameObject.CompareTag("Enemy"))
                    ApplyElementalVFX(element);
            }
            else if(taggedElement != ElementType.Normal && taggedElement != element)    // entity is already tagged and they were hit with different element, start a reaction
            {
                Reaction(element, damage);
            }
        }
        else
        {
            currentHealth = newHealth;
            ShowDamageNumber((int)damage, ElementType.Normal);
        }

        if(gameObject.CompareTag("Enemy"))
        {
            switch(element)
            {
                case ElementType.Zoom:
                    currentHitAttackVFX = Instantiate(zoomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor);
                    break;
                case ElementType.Boom:
                    currentHitAttackVFX = Instantiate(boomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor);
                    break;
                case ElementType.Gloom:
                    currentHitAttackVFX = Instantiate(gloomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor);
                    break;
            }
            
        }

        //if (OnHealthUpdatedEvent != null) OnHealthUpdatedEvent(currentHealth, maxHealth, taggedElement);
        OnHealthUpdatedEvent?.Invoke(currentHealth, maxHealth, taggedElement);
        OnEntityHurtEvent?.Invoke();

        //Instantiate(damageNumberVFXPrefab, transform);
    }

    private void ShowDamageNumber(int damage, ElementType element)
    {
        DamageNumber damageNumber = Instantiate(damageNumberVFXPrefab, transform.position, Quaternion.identity).GetComponent<DamageNumber>();
        damageNumber.ShowDamage(damage, element, damageNumberDisplayTime);
    }


    private void ApplyElementalVFX(ElementType element)
    {
        ClearVFX(ref currentElementalVFXInstance);
        if (element == ElementType.Zoom)
        {
            currentElementalVFXInstance = Instantiate(zoomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }
        else if (element == ElementType.Boom)
        {
            currentElementalVFXInstance = Instantiate(boomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }
        else if (element == ElementType.Gloom)
        {
            currentElementalVFXInstance = Instantiate(gloomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
        }
    }

    private void ClearVFX(ref GameObject instance)
    {
        if(instance != null)
        {
            Destroy(instance);
            instance = null;
        }
    }



    /// <summary>
    /// Signaled when a reaction occurs between two different elements
    /// </summary>
    /// <param name="initiatingElement">The second element that caused the reaction.</param>
    /// <param name="incomingDmg">The amount of damage being delt to the enemy before reaction.</param>
    private void Reaction( ElementType initiatingElement, float incomingDmg)
    {  
        // ZOOM x BOOM // Dmg multiplier
        if((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Boom || initiatingElement == ElementType.Boom))
        {
            float newHealth = currentHealth - (incomingDmg * dmgMultiplier);
            ShowDamageNumber((int)(incomingDmg * dmgMultiplier), initiatingElement);
            taggedElement = defaultElement;
            Instantiate(boomZoomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
            if(newHealth <= 0)
            {
                EntityHasDied();
                return;
            }
            // If entity survived, set new health and reset their tagged element to default
            currentHealth = newHealth;
        }
        // ZOOM x GLOOM  // Slow
        else if((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Gloom || initiatingElement == ElementType.Gloom))
        {
            currentHealth -= incomingDmg;
            ShowDamageNumber((int)incomingDmg, initiatingElement);
            taggedElement = defaultElement; // Reset tag to default/starting element
            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                Instantiate(gloomZoomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
                effectable.ApplySlow(data);
            }
        }
        // BOOM x GLOOM  // DOT 
        else if((taggedElement == ElementType.Boom || initiatingElement == ElementType.Boom) && (taggedElement == ElementType.Gloom || initiatingElement == ElementType.Gloom))
        {
            currentHealth -= incomingDmg;
            ShowDamageNumber((int)incomingDmg, initiatingElement);
            taggedElement = defaultElement; // Reset tag to default/starting element
            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                Instantiate(boomGloomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
                effectable.ApplyDot(data);
            }
        }

        if (gameObject.CompareTag("Enemy"))
            ClearVFX(ref currentElementalVFXInstance);

    } 

    /// <summary>
    /// Has the entity attack using their weapon if they aren't currently using an ability
    /// </summary>
    /// <param name="attackDirection">The direction the attack is facing. If the entity uses a melee wepaon, this parameter is unimportant.</param>
    /// <param name="entityPosition">The attacking entity's position. If the entity uses a melee weapon, this parameter is unimportant.</param>
    public void Attack(Vector3 attackDirection, Vector3 entityPosition)
    {
        if (AbilityInUse())return;
        
        if (weapon is RangedWeapon) 
            (weapon as RangedWeapon).UpdateWeaponTransform(attackDirection, entityPosition);
        
        bool attacked = weapon.Attack();
        if(attacked)
        {
            animator.SetTrigger("Shoot");
            if(gameObject.CompareTag("Enemy")) return;
            if(entityName == "Zoom")
            {
                if(vfxAnchor.childCount > 0)
                {
                    for(int i = 0; i < vfxAnchor.childCount; i++)
                    {
                        Destroy(vfxAnchor.GetChild(i).gameObject);
                    }
                }
                else
                {
                    currentZoomAttackVFX = Instantiate(zoomAttackVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
                }   
            }
        }
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

    /// <summary>
    /// Applies an attack rate mutliplier to this entity's weapon
    /// </summary>
    /// <param name="multiplier">The multiplier to apply. Make this > 1 to decrease attack rate and < 1 to increase </param>
    public void ApplyAttackCooldownMutliplier(float multiplier)
    {
        weapon.ApplyCooldownMultiplier(multiplier);
    }

    /// <summary>
    /// Restore this entity's attack rate to its base amount
    /// </summary>
    public void ResetAttackRate()
    {
        weapon.RestoreBaseFireRate();
    }





    // Healing / Entity Death =======================================================================================================================
    /// <summary>
    /// Called when the entity's health has reached 0 or less
    /// </summary>
    private void EntityHasDied()
    {
        currentHealth = 0;
        OnHealthUpdatedEvent?.Invoke(currentHealth, maxHealth, taggedElement);
        Debug.Log("Entity has died.");
        isAlive = false;
        ClearVFX(ref currentElementalVFXInstance);
        OnEntityKilledEvent?.Invoke();
        if (this.gameObject.CompareTag("Enemy"))
        {
            SpawnHealthPack spawnPack = GetComponent<SpawnHealthPack>();
            spawnPack.Spawn(gameObject.transform.position);
            Vector3 lastEnemyPosition = gameObject.transform.position;
            currentEnemyDeathVFX = Instantiate(enemyDeathVFX, lastEnemyPosition, Quaternion.identity);
            Destroy(gameObject); 
        }

        if (this.gameObject.CompareTag("Player"))
        {
            swappingManager.PlayerHasDied(gameObject);
        }
    }


    public void Heal(float heal)
    {
        isAlive = true;
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Entity healed. Current health: " + currentHealth);
        if(OnHealthUpdatedEvent != null) OnHealthUpdatedEvent(currentHealth, maxHealth, taggedElement);
    }
    
}
