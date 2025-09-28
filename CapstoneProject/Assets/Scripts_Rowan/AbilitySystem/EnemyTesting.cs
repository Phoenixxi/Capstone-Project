using UnityEngine;
using lilGuysNamespace;

public class EnemyTesting : MonoBehaviour, IEffectable
{
    EntityManager entityManager;
    private float movementSpeed = 2f;
    private Vector3 startPosition;
    public bool shouldMove = false;

    private AbilityData data;
    

    private void Start()
    {
        entityManager = GetComponent<EntityManager>();
        startPosition = transform.position;
    }

    void Update()
    {
        if(data != null)
        {
            HandleEffect();
        }

        if(shouldMove)
            HandleMove();
    }

    public bool moveRight = true;

    void HandleMove()
    {
        if(moveRight && Vector3.Distance(transform.position, startPosition + (transform.right * 3f)) < 0.01)
            moveRight = false;
        
        if(!moveRight && Vector3.Distance(transform.position, startPosition + (-transform.right * 3f)) < 0.01)
            moveRight = true;

        if(moveRight)
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        else
            transform.position += -transform.right * movementSpeed * Time.deltaTime;
    }


    private GameObject effectParticles;

    public void ApplyEffect(AbilityData data)
    {
        this.data = data;
        // PARTICLE EFFECTS HERE

        if(effectParticles != null)
            Destroy(effectParticles);
        effectParticles = Instantiate(data.ParticleEffects, transform);

        var ps = effectParticles.GetComponent<ParticleSystem>();
        if(ps != null)
        {
            Destroy(effectParticles, ps.main.duration + ps.main.startLifetime.constantMax);
        }
    } 

    private float currentEffectTime = 0f;
    private float lastTickTime = 0f;

    public void RemoveEffect()
    {
        data = null;
        currentEffectTime = 0f;
        lastTickTime = 0f;
        
        if(effectParticles != null)
        {
            Destroy(effectParticles);
            effectParticles = null;
        }
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
