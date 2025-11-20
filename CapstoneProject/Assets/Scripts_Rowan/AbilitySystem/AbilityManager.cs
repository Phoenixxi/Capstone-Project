using UnityEngine;
using UnityEngine.AI;
using lilGuysNamespace;

public class AbilityManager : MonoBehaviour, IEffectable
{
    EntityManager entityManager;
    UnityEngine.AI.NavMeshAgent navMeshAgent;
   // private float movementSpeed = 2f;
    private Vector3 startPosition;      
    private float startingMovementSpeed;    // do not change
    private float currentMovementSpeed;     // adjustable
    private bool applyingSlow = false;
    private bool applyingDOT = false;

    private AbilityData data;
    private AbilityData slowData;
    
    //private GameObject effectParticles;

    private void Start()
    {
        entityManager = GetComponent<EntityManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        startingMovementSpeed = navMeshAgent.speed; // store original speed
        currentMovementSpeed = startingMovementSpeed;
        startPosition = transform.position;
    }

    void Update()
    {
        if(data != null && applyingDOT)
            HandleEffect();

        if(data != null && applyingSlow)
            HandleSlow();
    }


    public void ApplyEffect(AbilityData data)
    {
        RemoveEffect();
        this.data = data;
        applyingDOT = true;
        HandleEffect();
       
    } 

    public void ApplySlow(AbilityData data)
    {
        RemoveEffect();
        this.data = data;
        applyingSlow = true;
        HandleSlow();
    }

    private float currentEffectTime = 0f;
    private float lastTickTime = 0f;

    public void RemoveEffect()
    {
        data = null;
        applyingDOT = false;
        applyingSlow = false;
        currentEffectTime = 0f;
        lastTickTime = 0f;
        navMeshAgent.speed = startingMovementSpeed;   // reset speed back to original
        currentMovementSpeed = startingMovementSpeed;
        
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
            entityManager.TakeDamage(data.DOTAmount, EntityData.ElementType.Normal);
        }
    }

    public void HandleSlow()
    {
        currentEffectTime += Time.deltaTime;
        if(currentEffectTime >= data.effectLifeTime)
        {   
            RemoveEffect();
            currentEffectTime = 0f;
        }

        if(data == null)
            return;

        if(data.movementPenalty > 0)
            currentMovementSpeed = startingMovementSpeed / data.movementPenalty;

        navMeshAgent.speed = currentMovementSpeed;
    }


   
}
