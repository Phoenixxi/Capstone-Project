using UnityEngine;

public class CombatStateExplode : CombatState
{
    public override void OnExit(AIContext aIContext)
    {
        base.OnExit(aIContext);
        EntityManager entityManager = aIContext.entityManagerEnemy;

        entityManager.CancelAttack();
    }

    public override AIStateType CheckTransition(AIContext aIContext)
    {
        if(StateCheck.CheckCombat(aIContext))
        {
            return AIStateType.Combat;
        }
        else
        {
            return AIStateType.Delay;
        }
    }
}
