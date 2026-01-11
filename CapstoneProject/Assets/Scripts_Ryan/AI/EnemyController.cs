using System;
using System.Collections.Generic;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public abstract class EnemyController : MonoBehaviour
{
    private AIContext aIContext;
    [Header("Enemy Attack Range. Default is 2.5 float")]
    [SerializeField] protected float AttackRange;
    [Header("Enemy Line Of Sight Range. \n The larger the number, the farther the player can be spotted. \n Default is 10")]
    [SerializeField] protected float LineOfSightRange;
    protected NavMeshAgent navMeshAgent; //The navmeshagent component that handles pathfinding
    protected GameObject player; //The player object that the enemy AI will path towards
    protected Dictionary<AIStateType, IState> stateDic = new Dictionary<AIStateType, IState>(); //A dictionary of all the states this enemy can be in
    protected IState CurrentState; //the current state the enemy is in
    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        navMeshAgent = GetComponent<NavMeshAgent>(); //Get the navmeshagent component from this enemy

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
        CurrentState = stateDic[AIStateType.Wandering];
    }

    protected void Start()
    {
        //initialize the AIContext
        initializeAIContext();
    }

    protected void Update()
    {
        //update the enemy with the current action and get the new state after doing said action
        IState newState = stateDic[CurrentState.UpdateAI(aIContext)];
        
        //check if we need to change states
        changeState(newState);
    }
    
    protected void changeState(IState newState)
    {
        if (newState.GetType() != CurrentState.GetType())
        {
            CurrentState.OnExit(aIContext);
            CurrentState = newState;
            CurrentState.OnEnter(aIContext);
        }
    }

    /// <summary>
    /// Change the speed at which the enemy moves at
    /// </summary>
    /// <param name="newVelocity"> The new speed the enemy should move at </param>
    protected void ChangeVelocity(float newVelocity)
    {
        navMeshAgent.speed = newVelocity;
    }

    /// <summary>
    /// Initializes the AIContext object with things the AI needs to know to make their own decisions
    /// </summary>
    protected void initializeAIContext()
    {
        aIContext = new AIContext(navMeshAgent, transform, player.transform, AttackRange, LineOfSightRange);
    }

    /// <summary>
    /// Initialize the dictionary with all the states this enemy can be in
    /// </summary>
    protected abstract void initializeStateDictionary();
}
