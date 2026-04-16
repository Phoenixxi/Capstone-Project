using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GrassChunk
{
    public Matrix4x4[] matrices;
}

[CreateAssetMenu(fileName = "GrassData", menuName = "ScriptableObjects/GrassData")]
public class GrassData : ScriptableObject
{
    // We are switching from one list to a list of chunks
    public List<GrassChunk> chunks = new List<GrassChunk>();
}