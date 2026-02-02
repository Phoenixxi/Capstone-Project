using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Collections;
using ElementType = lilGuysNamespace.EntityData.ElementType;
using UnityEngine.AI;
using System;

public class RuiEntityManager : MonoBehaviour
{
    [SerializeField] public string entityName;
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public ElementType defaultElement;
    [SerializeField] public ElementType taggedElement = ElementType.Normal;
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

    [Header("Ranged Weapon Settings")]
    [SerializeField] private GameObject projectile;

    [Header("Melee Weapon Settings")]
    [SerializeField] private Hurtbox meleeHurtbox;
    [SerializeField] private float hurtboxActivationTime;

    private Weapon weapon;
    private Vector3 movementVelocity;
    private Ability ability;
    private Queue<AbilityMovement> movementQueue;

    private float dmgMultiplier = 2.0f;

    [SerializeField] private GameObject damageNumberVFXPrefab;
    [SerializeField] private float damageNumberDisplayTime;

    [Header("VFX")]
    [SerializeField] private GameObject zoomElementVFX;
    [SerializeField] private GameObject boomElementVFX;
    [SerializeField] private GameObject gloomElementVFX;
    [SerializeField] private GameObject zoomDeathVFX;
    [SerializeField] private GameObject boomDeathVFX;
    [SerializeField] private GameObject gloomDeathVFX;
    [SerializeField] private GameObject zoomAttackVFX;
    [SerializeField] private GameObject boomJumpVFX;
    [SerializeField] public GameObject boomGloomReactionVFX;
    [SerializeField] private GameObject boomZoomReactionVFX;
    [SerializeField] private GameObject gloomZoomReactionVFX;
    [SerializeField] private GameObject zoomHitVFX;
    [SerializeField] private GameObject boomHitVFX;
    [SerializeField] private GameObject gloomHitVFX;
    [SerializeField] private GameObject enemyDeathVFX;

    private GameObject currentElementalVFXInstance;
    private GameObject currentZoomAttackVFX;
    private GameObject currentHitAttackVFX;
    private GameObject currentEnemyDeathVFX;
    private Transform vfxAnchor;
    private Transform vfxHitAnchor;

    public Action<float, float, ElementType> OnHealthUpdatedEvent;
    public Action OnEntityHurtEvent;
    public Action OnJumpEvent;
    public Action OnEntityKilledEvent;
    public Action<int> OnElementReactionEvent;
    public Action<ElementType> OnEntityAttackEvent;

    void Start()
    {
        currentHealth = maxHealth;
        // 如果忘记拖拽，自动获取
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        CreateWeapon();
        ability = GetComponent<Ability>();
        movementQueue = new Queue<AbilityMovement>();

        if (defaultElement != ElementType.Normal)
            taggedElement = defaultElement;
        if (GetComponent<NavMeshAgent>() != null) usesNavAgent = true;
    }

    void Awake()
    {
        vfxAnchor = transform.Find("VFXanchor");
        if (gameObject.CompareTag("Enemy"))
            vfxHitAnchor = transform.Find("VFXHitAnchor");
    }

    void Update()
    {
        if (movementQueue.Count > 0) HandleQueueMovement();
        else HandleDefaultMovement();

        // 只有落地时才重置跳跃次数
        if (entityMovement.isGrounded) currentExtraJumps = extraJumps;
    }

    public void SetHealthToFull()
    {
        currentHealth = maxHealth;
    }

    private void CreateWeapon()
    {
        if (projectile != null) weapon = new RangedWeapon(attackCooldown, weaponDamage, defaultElement, projectile, animator);
        else if (meleeHurtbox != null) weapon = new MeleeWeapon(attackCooldown, weaponDamage, defaultElement, meleeHurtbox, hurtboxActivationTime, animator);
        else Debug.LogError($"Neither a melee nor ranged weapon could be assigned to {gameObject}.");
    }

    // ==========================================
    // 🔥 核心修复区域：HandleDefaultMovement
    // ==========================================
    private void HandleDefaultMovement()
    {
        if (usesNavAgent) return;

        // 处理重力
        if (entityMovement.isGrounded && movementVelocity.y < 0)
            movementVelocity.y = -2f;
        movementVelocity.y -= gravity * Time.deltaTime;

        // 动画参数
        animator.SetFloat("Speed", new Vector2(movementVelocity.x, movementVelocity.z).magnitude); // 修复：只计算水平速度，忽略Y轴重力对跑动动画的影响
        animator.SetFloat("JumpVelocity", movementVelocity.y);
        animator.SetBool("isJumpingUp", movementVelocity.y > 0);
        animator.SetBool("isJumpingDown", movementVelocity.y < -3f);

        // --- 修复开始：翻转逻辑 ---
        // 使用 Scale 翻转而不是 flipX，这样子物体（箭头、攻击点）也会跟着翻转
        if (Mathf.Abs(movementVelocity.x) > 0.1f)
        {
            Vector3 scale = transform.localScale;
            // 如果速度为负(向左)，Scale X 设为负；速度为正(向右)，Scale X 设为正
            // Mathf.Sign(-5) = -1, Mathf.Sign(5) = 1
            scale.x = Mathf.Sign(movementVelocity.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        // 如果速度接近 0 (没按键)，这里会被跳过，Scale 保持不变 -> 也就不会自动向右了
        // --- 修复结束 ---

        entityMovement.Move(movementVelocity * Time.deltaTime);
    }

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

    public Vector3 GetMovementVelocity() => movementVelocity;

    public void SetMovementVelocity(Vector3 newVelocity) => movementVelocity = newVelocity;

    public void SetInputDirection(Vector2 input)
    {
        movementVelocity.x = input.x * movementSpeed;
        movementVelocity.z = input.y * movementSpeed;
    }

    public void Jump()
    {
        if (!entityMovement.isGrounded)
        {
            if (currentExtraJumps == 0) return;
            if (boomJumpVFX != null) Instantiate(boomJumpVFX, vfxAnchor.position, Quaternion.identity);
            currentExtraJumps--;
        }
        movementVelocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
        OnJumpEvent?.Invoke();
    }

    public void UseAbility(Vector2 horizontalDirection)
    {
        if (ability != null)
        {
            AbilityMovement[] movementList = ability.UseAbility(horizontalDirection);
            foreach (AbilityMovement movement in movementList) movementQueue.Enqueue(movement);
        }
        else
        {
            Debug.LogError("No ability assigned.");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        ShowDamageNumber((int)damage, ElementType.Normal);
        OnHealthUpdatedEvent?.Invoke(currentHealth, maxHealth, taggedElement);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            EntityHasDied();
            isAlive = false;
            return;
        }
        OnEntityHurtEvent?.Invoke();
    }

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
            if (taggedElement == ElementType.Normal || taggedElement == element)
            {
                taggedElement = element;
                currentHealth = newHealth;
                ShowDamageNumber((int)damage, element);
                if (gameObject.CompareTag("Enemy")) ApplyElementalVFX(element);
            }
            else if (taggedElement != ElementType.Normal && taggedElement != element)
            {
                Reaction(element, damage);
            }
        }
        else
        {
            currentHealth = newHealth;
            ShowDamageNumber((int)damage, ElementType.Normal);
        }

        if (gameObject.CompareTag("Enemy"))
        {
            switch (element)
            {
                case ElementType.Zoom: if (zoomHitVFX) currentHitAttackVFX = Instantiate(zoomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor); break;
                case ElementType.Boom: if (boomHitVFX) currentHitAttackVFX = Instantiate(boomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor); break;
                case ElementType.Gloom: if (gloomHitVFX) currentHitAttackVFX = Instantiate(gloomHitVFX, vfxHitAnchor.position, Quaternion.identity, vfxHitAnchor); break;
            }
        }
        OnHealthUpdatedEvent?.Invoke(currentHealth, maxHealth, taggedElement);
        OnEntityHurtEvent?.Invoke();
    }

    private void ShowDamageNumber(int damage, ElementType element)
    {
        if (damageNumberVFXPrefab != null)
        {
            DamageNumber damageNumber = Instantiate(damageNumberVFXPrefab, transform.position, Quaternion.identity).GetComponent<DamageNumber>();
            damageNumber.ShowDamage(damage, element, damageNumberDisplayTime);
        }
    }

    private void ApplyElementalVFX(ElementType element)
    {
        ClearVFX(ref currentElementalVFXInstance);
        if (element == ElementType.Zoom && zoomElementVFX) currentElementalVFXInstance = Instantiate(zoomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
        else if (element == ElementType.Boom && boomElementVFX) currentElementalVFXInstance = Instantiate(boomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
        else if (element == ElementType.Gloom && gloomElementVFX) currentElementalVFXInstance = Instantiate(gloomElementVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
    }

    private void ClearVFX(ref GameObject instance)
    {
        if (instance != null) { Destroy(instance); instance = null; }
    }

    private void Reaction(ElementType initiatingElement, float incomingDmg)
    {
        if ((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Boom || initiatingElement == ElementType.Boom))
        {
            float newHealth = currentHealth - (incomingDmg * dmgMultiplier);
            ShowDamageNumber((int)(incomingDmg * dmgMultiplier), initiatingElement);
            taggedElement = defaultElement;
            if (boomZoomReactionVFX) Instantiate(boomZoomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
            if (newHealth <= 0) { EntityHasDied(); return; }
            currentHealth = newHealth;
            OnElementReactionEvent?.Invoke(1);
        }
        else if ((taggedElement == ElementType.Zoom || initiatingElement == ElementType.Zoom) && (taggedElement == ElementType.Gloom || initiatingElement == ElementType.Gloom))
        {
            currentHealth -= incomingDmg;
            ShowDamageNumber((int)incomingDmg, initiatingElement);
            taggedElement = defaultElement;
            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                if (gloomZoomReactionVFX) Instantiate(gloomZoomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
                effectable.ApplySlow(data);
            }
            OnElementReactionEvent?.Invoke(2);
        }
        else if ((taggedElement == ElementType.Boom || initiatingElement == ElementType.Boom) && (taggedElement == ElementType.Gloom || initiatingElement == ElementType.Gloom))
        {
            currentHealth -= incomingDmg;
            ShowDamageNumber((int)incomingDmg, initiatingElement);
            taggedElement = defaultElement;
            var effectable = gameObject.GetComponent<IEffectable>();
            if (effectable != null && data != null)
            {
                if (boomGloomReactionVFX) Instantiate(boomGloomReactionVFX, vfxAnchor.position, Quaternion.identity, vfxAnchor);
                effectable.ApplyDot(data);
            }
            OnElementReactionEvent?.Invoke(3);
        }
        if (gameObject.CompareTag("Enemy")) ClearVFX(ref currentElementalVFXInstance);
    }

    public void Attack(Vector3 attackDirection, Vector3 entityPosition)
    {
        if (AbilityInUse()) return;
        if (weapon is RangedWeapon) (weapon as RangedWeapon).UpdateWeaponTransform(attackDirection, entityPosition);

        bool attacked = weapon.Attack();
        if (attacked)
        {
            OnEntityAttackEvent?.Invoke(defaultElement);
            animator.SetTrigger("Shoot");
            if (gameObject.CompareTag("Enemy")) return;
            if (entityName == "Zoom")
            {
                if (vfxAnchor.childCount > 0) { for (int i = 0; i < vfxAnchor.childCount; i++) Destroy(vfxAnchor.GetChild(i).gameObject); }
                else if (zoomAttackVFX) currentZoomAttackVFX = Instantiate(zoomAttackVFX, vfxAnchor.position, Quaternion.identity);
            }
        }
    }

    public bool AbilityInUse()
    {
        if (ability == null) return false;
        return ability.AbilityInUse();
    }

    public void ApplyAttackCooldownMutliplier(float multiplier) => weapon.ApplyCooldownMultiplier(multiplier);
    public void ResetAttackRate() => weapon.RestoreBaseFireRate();

    private void EntityHasDied()
    {
        currentHealth = 0;
        OnHealthUpdatedEvent?.Invoke(currentHealth, maxHealth, taggedElement);
        isAlive = false;
        ClearVFX(ref currentElementalVFXInstance);
        OnEntityKilledEvent?.Invoke();
        if (this.gameObject.CompareTag("Enemy"))
        {
            SpawnHealthPack spawnPack = GetComponent<SpawnHealthPack>();
            if (spawnPack) spawnPack.Spawn(gameObject.transform.position);
            if (enemyDeathVFX) currentEnemyDeathVFX = Instantiate(enemyDeathVFX, gameObject.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        if (this.gameObject.CompareTag("Player"))
        {
            Vector3 lastPos = gameObject.transform.position;
            switch (entityName)
            {
                case "Zoom": if (zoomDeathVFX) Instantiate(zoomDeathVFX, lastPos, Quaternion.identity); break;
                case "Boom": if (boomDeathVFX) Instantiate(boomDeathVFX, lastPos, Quaternion.identity); break;
                case "Gloom": if (gloomDeathVFX) Instantiate(gloomDeathVFX, lastPos, Quaternion.identity); break;
            }
            if (swappingManager) swappingManager.PlayerHasDied(gameObject);
        }
    }

    public void Heal(float heal)
    {
        isAlive = true;
        currentHealth += heal;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (OnHealthUpdatedEvent != null) OnHealthUpdatedEvent(currentHealth, maxHealth, taggedElement);
    }
}