using UnityEngine;

public class WanderingStateExplode : WanderingState
{
    public WanderingStateExplode(float range = 3f, bool hovering = false)
    {
        this.range = range;
        this.hovering = hovering;
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
