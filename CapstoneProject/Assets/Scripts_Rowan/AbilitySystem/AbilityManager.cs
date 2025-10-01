using UnityEngine;
using lilGuysNamespace;

public class AbilityManager : MonoBehaviour, IEffectable
{
    EntityManager entityManager;
    private float movementSpeed = 2f;
    private Vector3 startPosition;
    private float currentMovementSpeed;

    private AbilityData data;
    private AbilityData slowData;
    
    //private GameObject effectParticles;

    private void Start()
    {
        entityManager = GetComponent<EntityManager>();
        startPosition = transform.position;
        currentMovementSpeed = movementSpeed;
    }

    void Update()
    {
        if(data != null)
            HandleEffect();
        

    }


    public void ApplyEffect(AbilityData data)
    {
        RemoveEffect();
        this.data = data;
        // PARTICLE EFFECTS HERE

        // if(data.movementPenalty > 0)
        //     currentMovementSpeed = movementSpeed / data.movementPenalty;

        // if(effectParticles != null)
        //     Destroy(effectParticles);
        // effectParticles = Instantiate(data.ParticleEffects, transform);

        // var ps = effectParticles.GetComponent<ParticleSystem>();
        // if(ps != null)
        // {
        //     Destroy(effectParticles, ps.main.duration + ps.main.startLifetime.constantMax);
        // }
    } 

    private float currentEffectTime = 0f;
    private float lastTickTime = 0f;

    public void RemoveEffect()
    {
        data = null;
        currentEffectTime = 0f;
        lastTickTime = 0f;
        currentMovementSpeed = movementSpeed;
        
        // if(effectParticles != null)
        // {
        //     Destroy(effectParticles);
        //     effectParticles = null;
        // }
    }


    public void HandleEffect()
    {
        currentEffectTime += Time.deltaTime;
        if(currentEffectTime >= data.effectLifeTime)
        {
            RemoveEffect();
            currentEffectTime = 0f;
        }

        if(data == null)
            return;

        if(data.DOTAmount != 0 && currentEffectTime > lastTickTime)
        {
            lastTickTime += data.tickSpeed;
            entityManager.TakeDamage(data.DOTAmount);
        }

    }


   
}
