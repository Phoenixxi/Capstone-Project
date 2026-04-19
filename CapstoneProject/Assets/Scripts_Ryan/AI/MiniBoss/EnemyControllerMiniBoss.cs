using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerMiniBoss : EnemyController
{
    protected override void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        navMeshAgent = GetComponent<NavMeshAgent>(); //Get the navmeshagent component from this enemy
        entityManager = GetComponent<EntityManager>(); //Get the entity manager component

        navMeshAgent.enabled = false;
        
        //if these variables are empty, then set them to default values
        if (AttackRange == 0)
        {
            AttackRange = 2.5f;
        }

        if (LineOfSightRange == 0)
        {
            LineOfSightRange = 10f;
        }

        //initialize the state dictionary
        initializeStateDictionary();

        //set the current state to be wandering
        CurrentState = stateDic[AIStateType.Delay];
    }
    protected override bool isGrounded()
    {
        if(CurrentState is JumpStateMiniBoss) return true;
        return base.isGrounded();
    }
    protected override void initializeStateDictionary()
    {
        stateDic.Add(AIStateType.ChargingJump, new ChargingJumpStateMiniBoss());
        stateDic.Add(AIStateType.Jump, new JumpStateMiniBoss());
        stateDic.Add(AIStateType.Delay, new DelayStateMiniBoss(5));
    }
}
