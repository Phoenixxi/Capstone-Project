using UnityEngine;

public class ChasingStateExplode : ChasingState
{
    public override AIStateType CheckTransition(AIContext aIContext)
    {
        if(StateCheck.CheckCombat(aIContext))
        {
            return AIStateType.Combat;
        }
        else
        {
            return AIStateType.Chasing;
        }
    }
}
