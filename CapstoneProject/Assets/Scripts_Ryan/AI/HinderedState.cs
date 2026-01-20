using UnityEngine;

/// <summary>
/// State that enemies enter when they are hindered. Being "hindered" refers to any time the enemy has
/// one or more movements they are required to complete before they can resume regular behavior
/// </summary>
public class HinderedState : IState
{   
    public AIStateType CheckTransition(AIContext aIContext)
    {
        //I didn't leave an option for the Delayed state since I figured if the enemy is already immobilized, there wouldn't
        //be much reason to freeze them again, but feel free to change if desired
        if(StateCheck.CheckHindered(aIContext))
        {
            return AIStateType.Hindered;
        }
        else if (StateCheck.CheckCombat(aIContext))
        {
            return AIStateType.Combat;
        }
        else if (StateCheck.CheckChasing(aIContext))
        {
            return AIStateType.Chasing;
        }
        else
        {
            return AIStateType.Wandering;
        }
    }

    public void OnEnter(AIContext aIContext)
    {
        aIContext.agent.ResetPath();
        Debug.Log("Enemy is hindered");
    }

    public void OnExit(AIContext aIContext)
    {
        Debug.Log("Enemy is no longer hindered");
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        return CheckTransition(aIContext);
    }
}
