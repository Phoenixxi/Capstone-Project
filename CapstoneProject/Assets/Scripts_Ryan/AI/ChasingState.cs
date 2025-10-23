using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class ChasingState : IState
{
    public void OnEnter(AIContext aIContext)
    {
        
    }

    public void OnExit(AIContext aIContext)
    {
        NavMeshAgent navMeshAgent = aIContext.agent;

        navMeshAgent.ResetPath();
    }

    public void UpdateAI(AIContext aIContext)
    {
        NavMeshAgent navMeshAgent = aIContext.agent;
        Transform player = aIContext.PlayerTransform;
        navMeshAgent.SetDestination(player.position);
    }
}
