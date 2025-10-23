using NUnit.Framework;
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

        //make the enemy look at the player
        EnemyTransform.LookAt(PlayerTransform);

        isAttacking = true; //lock the enemy in attack state
        entityManager.Attack(EnemyTransform.forward, EnemyTransform.position); //call attack
        Debug.Log("CombatState Attacking");
        isAttacking = false; //unlock the enemy from attack state
    }
}
