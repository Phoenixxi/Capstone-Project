using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Scriptable Objects/SoundLibrary")]
public class SoundLibrary : ScriptableObject
{
    public AudioManager.SoundEffect[] soundList;

    private void OnEnable()
    {
        foreach (var sound in soundList) sound.lastUsedTime = 0f;
    }
}
