using UnityEngine;

/// <summary>
/// A simple class that represents special movement that must be executed for an ability
/// </summary>
public class AbilityMovement
{
    protected Vector3 movement;
    protected bool hasEnded;
    

    public AbilityMovement(Vector3 movement)
    {
        this.movement = movement;
        hasEnded = false;
    }

    /// <summary>
    /// Returns the movement velocity as a Vector3
    /// </summary>
    /// <returns></returns>
    public Vector3 GetMovementVelocity()
    {
        return movement;
    }

    /// <summary>
    /// Returns whether or not this movement has ended
    /// </summary>
    /// <returns></returns>
    public virtual bool HasEnded()
    {
        return hasEnded;
    }

    /// <summary>
    /// Marks this ability movement as complete
    /// </summary>
    public void Complete()
    {
        hasEnded = true;
    }
}
