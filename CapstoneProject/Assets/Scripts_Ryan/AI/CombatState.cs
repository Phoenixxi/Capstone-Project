
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public abstract class CombatState : IState
{

    public virtual void OnEnter(AIContext aIContext)
    {
        
    }

    public virtual void OnExit(AIContext aIContext)
    {

    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        var animationState = aIContext.animator.GetCurrentAnimatorStateInfo(0);
        bool isAttacking = animationState.IsTag("Attack");
        if(isAttacking && animationState.normalizedTime < 1f)
        {
            aIContext.agent.velocity = Vector3.zero;
            aIContext.agent.ResetPath();
            return AIStateType.Combat;
        }

        //Grab necessary references
        EntityManager entityManager = aIContext.entityManagerEnemy;
        Transform EnemyTransform = aIContext.EnemyTransform;
        Transform PlayerTransform = aIContext.PlayerTransform;
        Vector3 direction = EnemyTransform.forward;

        //make the enemy look at the player
        if (aIContext.DistanceToPlayer > 0.5f)
        {
            direction = (PlayerTransform.position - EnemyTransform.position).normalized;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                EnemyTransform.rotation = Quaternion.Slerp(EnemyTransform.rotation, targetRotation, Time.deltaTime * 5f);
            }
        }

        if (!isAttacking)
        {
            entityManager.Attack(direction, EnemyTransform.position); //call attack
        }

        return CheckTransition(aIContext);
    }

    public abstract AIStateType CheckTransition(AIContext aIContext);
}
