using UnityEngine;

public class WanderStateTutorialChasingEnemy : WanderingState
{
    public WanderStateTutorialChasingEnemy(float range = 3f, bool hovering = false)
    {
        this.range = range;
        this.hovering = hovering;
    }

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
