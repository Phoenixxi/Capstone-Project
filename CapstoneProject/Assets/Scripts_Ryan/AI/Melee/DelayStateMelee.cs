using UnityEngine;

public class DelayStateMelee : DelayState
{
    public DelayStateMelee(float DelaySeconds)
    {
        this.DelaySeconds = DelaySeconds;
    }

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
