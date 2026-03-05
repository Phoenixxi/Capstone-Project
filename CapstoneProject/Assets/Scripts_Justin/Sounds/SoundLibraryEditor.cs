#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(SoundLibrary))]
public class SoundLibraryEditor : Editor
{
    private void OnEnable()
    {
        ref AudioManager.SoundEffect[] sounds = ref ((SoundLibrary)target).soundList;
        string[] soundNames = Enum.GetNames(typeof(SoundName));
        Array.Resize(ref sounds, soundNames.Length);
        for (int i = 0; i < soundNames.Length; i++)
        {
            //sounds[i].name = soundNames[i];
            sounds[i].name = soundNames[i];
        }
    }
}
#endif
