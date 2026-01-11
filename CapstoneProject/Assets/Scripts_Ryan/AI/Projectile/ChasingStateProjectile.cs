using UnityEngine;
using UnityEngine.Animations;

public class ChasingStateProjectile : ChasingState
{
    public override AIStateType CheckTransition(AIContext aIContext)
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
}
