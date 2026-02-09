using System.Collections;
using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem.DualShock.LowLevel;
using static lilGuysNamespace.EntityData;

public class ExplodeHurtBox : MonoBehaviour
{
    [SerializeField] private float screenShakeIntensity;
    [SerializeField] private float screenShakeDuration;
    [SerializeField] private LayerMaskType layerMaskType;
    private ElementType element;
    private float damage;
    [SerializeField] private SphereCollider explodeCollider;
    [SerializeField] private float duration = 0.5f;
    private CameraController cameraController;
    private LayerMask layerMask;

    private void Awake()
    {
        explodeCollider.enabled = false;
        cameraController = FindFirstObjectByType<CameraController>();

        switch (layerMaskType)
        {
            default:
                layerMask = LayerMask.GetMask("Enemy");
                break;
            case LayerMaskType.EnemyLayerMask:
                layerMask = LayerMask.GetMask("Player");
                break;
        }

        enabled = false;
    }

    void OnEnable()
    {
        StartCoroutine(Attack());
    }

    public void Activate()
    {
        enabled = true;
    }

    public IEnumerator Attack()
    {
        float time = 0f;
        bool hashit = false;

        while(time < duration)
        {
            Collider[] colliders = Physics.OverlapSphere(explodeCollider.bounds.center, explodeCollider.radius, layerMask);
        
            foreach (Collider collider in colliders)
            {
                Debug.Log($"Melee hit {collider.gameObject}", collider.gameObject);
                EntityManager entityManager = collider.gameObject.GetComponentInChildren<EntityManager>();
                if(entityManager == null)
                {
                    Debug.LogError("oh thats not good brother, there should be an entitymanager");
                }
                entityManager.TakeDamage(damage, element);
            
                hashit = true;
                time = duration;
            }

            if(time >= duration)
            {
                continue;
            }
            time += Time.deltaTime;

            yield return null;
        }

        Debug.Log("Explode enemy attacked");

        if(hashit)
        {
            cameraController.ShakeCamera(screenShakeIntensity, screenShakeDuration);
        }
    }

    public void SetHurtboxDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetElementType(ElementType elementType)
    {
        this.element = elementType;
    }


}
