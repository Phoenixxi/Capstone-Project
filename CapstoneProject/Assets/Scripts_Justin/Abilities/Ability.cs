using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Base class used by all abilities that entities can use
/// </summary>
public abstract class Ability : MonoBehaviour
{
    [Header("Universal Ability Settings")]
    [SerializeField] protected float cooldown;
    [SerializeField] protected int damage;
    [SerializeField] protected ElementType element;
    [SerializeField] protected float screenShakeIntensity;
    [SerializeField] protected float screenShakeDuration;

    protected bool abilityInUse;
    protected float currentCooldown;
    protected AbilityMovement[] movements;
    protected float disabledTime = 0f;
    protected CameraController cameraController;

    protected virtual void Awake()
    {
        abilityInUse = false;
        currentCooldown = 0f;
        cameraController = FindFirstObjectByType<CameraController>();
    }

    protected virtual void Update()
    {
        if (!abilityInUse && currentCooldown > 0) currentCooldown -= Time.deltaTime;
        else currentCooldown = 0f;
    }

    public bool AbilityInUse()
    {
        return abilityInUse;
    }

    protected void OnDisable()
    {
        disabledTime = Time.time;
    }

    protected void OnEnable()
    {
        float lostTIme = Time.time - disabledTime;
        if (currentCooldown > 0f) currentCooldown -= lostTIme;
    }

    /// <summary>
    /// Executes this ability if it is not on cooldown and returns the movement(s) associated with the activation
    /// </summary>
    /// <param name="horizontalDirection">Normalized vector that indicates the entity's horizontal movement direction at the time of activation</param>
    /// <returns>An array of AbilityMovements that are associated with this ability</returns>
    public abstract AbilityMovement[] UseAbility(Vector2 horizontalDirection);
}
