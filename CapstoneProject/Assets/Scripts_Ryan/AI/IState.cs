using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    /// <summary>
    /// Method to play when the enemy AI transitions into this state
    /// Examples would be:
    /// "Play spawning Animation"
    /// </summary>
    void OnEnter(AIContext aIContext);

    /// <summary>
    /// Method to play when the enemy AI transitions out of this state
    /// Examples would be:
    /// "Play death animation"
    /// </summary>
    void OnExit(AIContext aIContext);

    /// <summary>
    /// Updates the AI with an action to perform
    /// based on the particular state class
    /// </summary>
    /// <param name="aIContext">Context about enemy AI conditions</param>
    /// <returns></returns>
    AIStateType UpdateAI(AIContext aIContext);

    AIStateType CheckTransition(AIContext aIContext);
}
