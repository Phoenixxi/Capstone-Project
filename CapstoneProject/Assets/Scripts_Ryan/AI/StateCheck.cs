using UnityEngine;
using UnityEngine.Animations;
using ElementType = lilGuysNamespace.EntityData.ElementType;

public enum AIStateType { Combat, Chasing, Wandering, Hover }
public static class StateCheck
{
    private static LayerMask layerMask = LayerMask.GetMask("Walls", "Player");
    public static AIStateType GetNewState(AIContext aIContext)
    {
        if (CheckCombat(aIContext)) return AIStateType.Combat;
        if (CheckChasing(aIContext)) return AIStateType.Chasing;
        return AIStateType.Wandering;
    }
    private static bool CheckCombat(AIContext aIContext)
    {
        Vector3 PlayerPosition = aIContext.PlayerTransform.position;
        Vector3 EnemyPosition = aIContext.EnemyTransform.position;
        float AttackRange = aIContext.AttackRange;

        Vector3 directionToPlayer = PlayerPosition - EnemyPosition;

        return RayCastToPlayer(EnemyPosition, directionToPlayer.normalized, AttackRange, layerMask);
    }

    private static bool CheckChasing(AIContext aIContext)
    {
        float DistanceToPlayer = aIContext.DistanceToPlayer;
        float LineOfSightRange = aIContext.LineOfSightRange;

        if (DistanceToPlayer > LineOfSightRange)
        {
            return false;
        }

        Vector3 PlayerPosition = aIContext.PlayerTransform.position;
        Vector3 EnemyPosition = aIContext.EnemyTransform.position;

        Vector3 directionToPlayer = PlayerPosition - EnemyPosition;

        return RayCastToPlayer(EnemyPosition, directionToPlayer.normalized, LineOfSightRange, layerMask);
    }

    private static bool RayCastToPlayer(Vector3 EnemyPos, Vector3 directionToPlayer, float Range, LayerMask layerMask)
    {
        RaycastHit raycastHit;
        if (Physics.Raycast(EnemyPos, directionToPlayer, out raycastHit, Range, layerMask))
        {
            if (raycastHit.collider.CompareTag("Player"))
            {
                return true;
            }

            return false;
        }
        return false;
    }
    
}
