using UnityEngine;
using UnityEngine.Pool;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Script that manages damage number "spawning" for when entities are hit
/// </summary>
public class DamageNumberManager : MonoBehaviour
{
    [SerializeField] private GameObject damageNumberVFX;
    [SerializeField] private int defaultSize = 10;
    [SerializeField] private int maxSize = 200;
    [SerializeField] private float displayTime;
    private int currentSize;

    private ObjectPool<DamageNumber> numberPool;

    private void Awake()
    {
        numberPool = new ObjectPool<DamageNumber>(createFunc: SpawnDamageNumber, actionOnDestroy: DestroyDamageNumber, defaultCapacity: defaultSize, maxSize: maxSize);
        currentSize = numberPool.CountAll;
    }

    /// <summary>
    /// Spawns in a damage number
    /// </summary>
    /// <returns>DamageNumber component of the game object</returns>
    private DamageNumber SpawnDamageNumber()
    {
        DamageNumber number = Instantiate(damageNumberVFX).GetComponent<DamageNumber>();
        number.ObjectPool = numberPool;
        return number;
    }

    /// <summary>
    /// Destroys a given damage number instance
    /// </summary>
    /// <param name="number">The damage number being destroyed</param>
    private void DestroyDamageNumber(DamageNumber number)
    {
        Destroy(number.gameObject);
    }

    /// <summary>
    /// Shows a damage number at the given position
    /// </summary>
    /// <param name="damage">The amount of damage to display</param>
    /// <param name="element">The displayed damage's element</param>
    /// <param name="position">The position the number should be shown at</param>
    public void ShowDamageNumber(int damage, ElementType element, Vector3 position)
    {
        DamageNumber number = numberPool.Get();
        //TODO Calculate how many other damage numbers there are
        number.ShowDamage(damage, element, displayTime, position);
    }
}
