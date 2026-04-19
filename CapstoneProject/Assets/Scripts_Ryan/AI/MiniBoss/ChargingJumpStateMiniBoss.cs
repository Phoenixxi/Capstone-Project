using UnityEngine;

public class ChargingJumpStateMiniBoss : IState
{
    public AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.Jump;
    }

    public void OnEnter(AIContext aIContext)
    {
        aIContext.animator.SetTrigger("ChargingJump");
    }

    public void OnExit(AIContext aIContext)
    {
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        var animationState = aIContext.animator.GetCurrentAnimatorStateInfo(0);

        if(animationState.IsName("MiniBoss_ChargingJump") && animationState.normalizedTime >= 1f)
        {
            return CheckTransition(aIContext);
        }

        return AIStateType.ChargingJump;
    }
}
