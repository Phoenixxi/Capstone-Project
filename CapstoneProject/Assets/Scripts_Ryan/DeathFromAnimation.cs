using UnityEngine;

public class DeathFromAnimation : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;

    void Awake()
    {
        entityManager = GetComponentInParent<EntityManager>();
    }

    public void DeathFromAnim()
    {
        entityManager.TakeDamage(entityManager.maxHealth);
    }
}
