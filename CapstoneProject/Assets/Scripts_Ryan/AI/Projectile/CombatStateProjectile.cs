using UnityEngine;

public class CombatStateProjectile : CombatState
{
    public override AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.Hover;
    }
}
