using UnityEngine;

public class ChaseStateTutorialChasingEnemy : ChasingState
{
    public override AIStateType CheckTransition(AIContext aIContext)
    {
        if(StateCheck.CheckChasing(aIContext))
        {
            return AIStateType.Chasing;
        }
        else
        {
            return AIStateType.Wandering;
        }
    }
}
