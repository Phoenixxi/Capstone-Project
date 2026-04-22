using UnityEngine;

public class AttackFromAnimation : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private GameObject Attack_VFX;
    [SerializeField] private Transform vfxAnchor;

    void Awake()
    {
        entityManager = GetComponentInParent<EntityManager>();
    }

    public void AttackFromAnim()
    {
        entityManager.AttackFromAnimation();

        if(Attack_VFX != null)
        {
            Instantiate(Attack_VFX, vfxAnchor.position, Quaternion.identity);
        }
    }
}
