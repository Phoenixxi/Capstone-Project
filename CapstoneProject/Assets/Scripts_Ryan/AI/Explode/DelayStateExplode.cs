using UnityEngine;

public class DelayStateExplode : DelayState
{
    public DelayStateExplode(float DelaySeconds)
    {
        this.DelaySeconds = DelaySeconds;
    }
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
