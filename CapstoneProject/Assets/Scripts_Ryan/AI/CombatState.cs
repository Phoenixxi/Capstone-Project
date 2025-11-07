
using NUnit.Framework;
using Unity.Mathematics;
using UnityEngine;

public class CombatState : IState
{
    public bool isAttacking = false; //variable to lock the enemy in attack state
    public void OnEnter(AIContext aIContext)
    {
        
    }

    public void OnExit(AIContext aIContext)
    {

    }

    public void UpdateAI(AIContext aIContext)
    {
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
            isAttacking = true; //lock the enemy in attack state
            entityManager.Attack(direction, EnemyTransform.position); //call attack
            //Debug.Log("CombatState Attacking");
            isAttacking = false; //unlock the enemy from attack state
        }
    }
}
