using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;

public class WanderingState : IState
{
    private float range;
    private bool hovering;

    public WanderingState(float range = 3f, bool hovering = false)
    {
        this.range = range;
        this.hovering = hovering;
    }

    public void OnEnter(AIContext aIContext)
    {

    }

    public void OnExit(AIContext aIContext)
    {
        //(OPTIONAL)
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        NavMeshAgent agent = aIContext.agent;
        Transform transform;
        if(hovering)
        {
            transform = aIContext.PlayerTransform;
        }
        transform = aIContext.EnemyTransform;
        
        //check if the enemy AI is done with its previous pathing
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            Vector3 point;
            if (RandomPoint(transform.position, range, out point))
            {
                agent.SetDestination(point);
            }
        }

        return CheckTransition(aIContext);
    }

    public AIStateType CheckTransition(AIContext aIContext)
    {
        if(StateCheck.CheckCombat(aIContext))
        {
            return AIStateType.Combat;
        }
        else if(StateCheck.CheckChasing(aIContext))
        {
            return AIStateType.Chasing;
        }
        else
        {
            return AIStateType.Wandering;
        }
    }

    /// <summary>
    /// Picks a random point for the AI to go to
    /// </summary>
    /// <param name="center">Center of the Circle (Can be something like player.position)</param>
    /// <param name="range">How far can the enemy AI go within the circle (Radius of the circle)</param>
    /// <param name="result">The point to where the enemy AI will go to</param>
    /// <returns></returns>
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 randomPoint = center + Random.insideUnitSphere * range; //random point in a sphere
        NavMeshHit hit;

        //Navmesh.SamplePosition finds the nearest navigable (walkable) point on the NavMesh
        //within a given radius
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            //if there is a point on the navmesh near randomPoint
            //that the AI can walk to, return that result
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }
}
