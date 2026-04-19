using UnityEngine;

public class DelayStateMiniBoss : DelayState
{
    public DelayStateMiniBoss(float seconds)
    {
        DelaySeconds = seconds;
    }

    public override AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.ChargingJump;
    }
}
