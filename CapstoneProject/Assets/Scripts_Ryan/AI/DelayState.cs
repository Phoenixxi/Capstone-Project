using UnityEngine;

public abstract class DelayState : IState
{
    protected float DelaySeconds;
    private float SecondsPassed = 0;
    public abstract AIStateType CheckTransition(AIContext aIContext);

    public void OnEnter(AIContext aIContext)
    {
        
    }

    public void OnExit(AIContext aIContext)
    {
        
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        if(SecondsPassed < DelaySeconds)
        {
            SecondsPassed++;
            return AIStateType.Delay;
        }

        SecondsPassed = 0;

        return CheckTransition(aIContext);
    }
}
