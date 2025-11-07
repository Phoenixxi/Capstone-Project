using System;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class EnemyController : MonoBehaviour
{
    private AIContext aIContext;
    [Header("Enemy Attack Range. Default is 2.5 float")]
    [SerializeField] private float AttackRange;
    [Header("Enemy Line Of Sight Range. \n The larger the number, the farther the player can be spotted. \n Default is 10")]
    [SerializeField] private float LineOfSightRange;
    private NavMeshAgent navMeshAgent; //The navmeshagent component that handles pathfinding
    private GameObject player; //The player object that the enemy AI will path towards
    private IState CurrentState;
    private IState wanderingState = new WanderingState();
    private IState chasingState = new ChasingState();
    private IState combatState = new CombatState();
    private IState hoveringState = new WanderingState(3f, true);
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        navMeshAgent = GetComponent<NavMeshAgent>(); //Get the navmeshagent component from this enemy

        if (AttackRange == 0)
        {
            AttackRange = 2.5f;
        }

        if (LineOfSightRange == 0)
        {
            LineOfSightRange = 10f;
        }

        CurrentState = new WanderingState();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initializeAIContext();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState.GetType() == typeof(CombatState))
        {
            CombatState cs = (CombatState)CurrentState;
            if (cs.isAttacking) return;
            
            ChangeUpdate(hoveringState);
            return;
        }

        IState newState;
        switch (StateCheck.GetNewState(aIContext))
        {
            default:
                newState = wanderingState;
                break;
            case AIStateType.Chasing:
                newState = chasingState;
                break;
            case AIStateType.Combat:
                newState = combatState;
                break;
            case AIStateType.Hover:
                newState = hoveringState;
                break;
        }

        ChangeUpdate(newState);
    }
    
    private void ChangeUpdate(IState newState)
    {
        if (newState.GetType() != CurrentState.GetType())
        {
            changeState(newState);
        }

        CurrentState.UpdateAI(aIContext);
    }
    
    private void changeState(IState new_state)
    {
        CurrentState.OnExit(aIContext);
        CurrentState = new_state;
        CurrentState.OnEnter(aIContext);
    }

    /// <summary>
    /// Change the speed at which the enemy moves at
    /// </summary>
    /// <param name="newVelocity"> The new speed the enemy should move at </param>
    public void ChangeVelocity(float newVelocity)
    {
        navMeshAgent.speed = newVelocity;
    }

    private void initializeAIContext()
    {
        aIContext = new AIContext(navMeshAgent, transform, player.transform, AttackRange, LineOfSightRange);
    }
}
