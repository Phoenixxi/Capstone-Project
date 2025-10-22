using NUnit.Framework;
using UnityEngine;

public class CombatState : IState
{
    public bool isAttacking = false;
    public void OnEnter(AIContext aIContext)
    {
        
    }

    public void OnExit(AIContext aIContext)
    {

    }

    public void UpdateAI(AIContext aIContext)
    {
        EntityManager entityManager = aIContext.entityManagerEnemy;
        Transform EnemyTransform = aIContext.EnemyTransform;

        isAttacking = true;
        entityManager.Attack(EnemyTransform.forward, EnemyTransform.position);
        Debug.Log("CombatState Attacking");
        isAttacking = false;
    }
}
