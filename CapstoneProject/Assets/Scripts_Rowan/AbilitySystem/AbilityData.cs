using UnityEngine;
using lilGuysNamespace;

[CreateAssetMenu(fileName = "AbilityData", menuName = "Scriptable Objects/AbilityData")]
public class AbilityData : ScriptableObject
{
    public string AbilityName;
    public float DOTAmount;
    public float tickSpeed;
    public float movementPenalty;
    public float effectLifeTime;
    public GameObject ParticleEffects; // might change this to particle system later
}
