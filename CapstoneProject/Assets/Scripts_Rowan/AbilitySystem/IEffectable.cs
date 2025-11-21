using UnityEngine;
using lilGuysNamespace;

public interface IEffectable
{
    public void ApplyDot(AbilityData data);
    public void ApplySlow(AbilityData data);
    public void RemoveEffect();
    public void HandleDot();
    public void HandleSlow();
}
