using UnityEngine;

public class CombatStateMelee : CombatState
{
    public override AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.Delay;
    }
}
