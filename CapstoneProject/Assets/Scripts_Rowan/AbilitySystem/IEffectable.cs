using UnityEngine;
using lilGuysNamespace;

public interface IEffectable
{
    public void ApplyEffect(AbilityData data);
    public void RemoveEffect();
    public void HandleEffect();
}
