using UnityEngine;

public class AttackFromAnimation : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;

    void Awake()
    {
        entityManager = GetComponentInParent<EntityManager>();
    }

    public void AttackFromAnim()
    {
        entityManager.AttackFromAnimation();
    }
}
