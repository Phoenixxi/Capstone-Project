using UnityEngine;

/// <summary>
/// Class representing attacks that can be performed by the boss. Meant to be instantiated in order to trigger the attack and is the destroyed when the attack concludes
/// </summary>
public abstract class BossAttack : MonoBehaviour
{
    [Header("General Attack Settings")]
    [SerializeField] protected int damage;
    [SerializeField] protected bool canRepeat; //Set to true if you don't want the attack to delete itself after finishing
    [SerializeField] protected float speedupAmount = 1f;
    [SerializeField] protected bool spawnSpedUp = false;
    public bool IsAttacking { get; protected set; }

    protected virtual void Start()
    {
        IsAttacking = false;
        if (spawnSpedUp) ApplySpeedup();
        if(!canRepeat) Attack();
    }

    /// <summary>
    /// Triggers this attack to occur
    /// </summary>
    public abstract void Attack();

    /// <summary>
    /// Makes this attack operate at an increased speed
    /// </summary>
    public abstract void ApplySpeedup();
}
