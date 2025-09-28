using UnityEngine;
using lilGuysNamespace;

public class EnemyTesting : MonoBehaviour, IEffectable
{
    EntityManager entityManager;
    private float movementSpeed = 2f;
    private Vector3 startPosition;
    public bool shouldMove = false;
    private float currentMovementSpeed;

    private AbilityData data;
    
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


        //var currentMoveSpeed = data == null ? movementSpeed : moveSpeed / data.movementPenalty;
        if(moveRight)
            transform.position += transform.right * currentMovementSpeed * Time.deltaTime;
        else
            transform.position += -transform.right * currentMovementSpeed * Time.deltaTime;
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
