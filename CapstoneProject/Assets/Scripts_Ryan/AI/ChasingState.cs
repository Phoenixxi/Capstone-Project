using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public abstract class ChasingState : IState
{
    public void OnEnter(AIContext aIContext)
    {
        
    }

    public void OnExit(AIContext aIContext)
    {
        NavMeshAgent navMeshAgent = aIContext.agent;

        navMeshAgent.ResetPath();
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        NavMeshAgent navMeshAgent = aIContext.agent;
        Transform player = aIContext.PlayerTransform;
        navMeshAgent.SetDestination(player.position);

        return CheckTransition(aIContext);
    }

    public abstract AIStateType CheckTransition(AIContext aIContext);

}
