using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewGrassData", menuName = "Grass/Data Container")]
public class GrassData : ScriptableObject
{
    public List<Matrix4x4> matrices = new List<Matrix4x4>();
}