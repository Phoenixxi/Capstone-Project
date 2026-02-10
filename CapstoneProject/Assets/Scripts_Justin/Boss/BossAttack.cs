using UnityEngine;

/// <summary>
/// Class representing attacks that can be performed by the boss. Meant to be instantiated in order to trigger the attack and is the destroyed when the attack concludes
/// </summary>
public abstract class BossAttack : MonoBehaviour
{
    [Header("General Attack Settings")]
    [SerializeField] protected int damage;
    
    protected virtual void Start()
    {
        Attack();
    }

    /// <summary>
    /// Triggers this attack to occur
    /// </summary>
    protected abstract void Attack();
}
