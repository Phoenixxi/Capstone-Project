using UnityEngine;

public class CombatStateExplode : CombatState
{
    public override AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.Delay;
    }
}
