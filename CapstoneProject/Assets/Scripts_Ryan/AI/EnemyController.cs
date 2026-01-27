using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.TextCore.Text;

public abstract class EnemyController : MonoBehaviour
{
    private AIContext aIContext;
    [Header("Enemy Attack Range. Default is 2.5 float")]
    [SerializeField] protected float AttackRange;
    [Header("Enemy Line Of Sight Range. \n The larger the number, the farther the player can be spotted. \n Default is 10")]
    [SerializeField] protected float LineOfSightRange;
    [SerializeField] protected CharacterController characterController;
    protected EntityManager entityManager; //The entity manager of this enemy
    protected NavMeshAgent navMeshAgent; //The navmeshagent component that handles pathfinding
    protected GameObject player; //The player object that the enemy AI will path towards
    protected Dictionary<AIStateType, IState> stateDic = new Dictionary<AIStateType, IState>(); //A dictionary of all the states this enemy can be in
    protected IState CurrentState; //the current state the enemy is in

    protected virtual void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //Find the player in the world
        navMeshAgent = GetComponent<NavMeshAgent>(); //Get the navmeshagent component from this enemy
        entityManager = GetComponent<EntityManager>(); //Get the entity manager component

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
        bool grounded = isGrounded();
        if(entityManager.movementQueue.Count > 0 || !grounded)
        {
            navMeshAgent.enabled = false;
            navMeshAgent.Warp(transform.position);
            if(!grounded)
                applyGravity();
            return;
        }

        navMeshAgent.enabled = true;
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

    protected bool isGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, characterController.height/2 + 0.1f, LayerMask.GetMask("Ground"));
    }

    protected void applyGravity()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - entityManager.getGravity() * Time.deltaTime, transform.position.z);
    }

    /// <summary>
    /// Initialize the dictionary with all the states this enemy can be in
    /// </summary>
    protected abstract void initializeStateDictionary();
}
