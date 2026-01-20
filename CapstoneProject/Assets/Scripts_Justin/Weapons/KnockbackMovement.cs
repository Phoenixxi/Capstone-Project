using UnityEngine;

/// <summary>
/// A class to represent special movements caused by attacks. Works identically to the AbilityMovement class, only it
/// is able to verify on its own whether or not it has completed. Can also be used to make knock-ups and stuns
/// </summary>
public class KnockbackMovement : AbilityMovement
{
    private float creationTime;
    private float duration;

    public KnockbackMovement(Vector3 movement, float creationTime, float duration) : base(movement)
    {
        this.creationTime = creationTime;
        this.duration = duration;
    }

    public override bool HasEnded()
    {
        float checkedTime = Time.time;
        if (checkedTime - creationTime >= duration) Complete();
        return base.HasEnded();
    }
}
