using System;
using UnityEngine;

/// <summary>
/// Simple class that holds data for sound effects
/// </summary>
[Serializable]
public class Sound
{
    [SerializeField]
    private AudioClip sound;
    [SerializeField]
    [Range(0, 1)] private float volume = 1f;
    [SerializeField] private float soundCooldown = 0f;
    private float lastPlayedTime = 0f;
    public AudioClip SoundClip { get => sound; }
    public float Volume { get => volume; }

    /// <summary>
    /// Checks to see if the sound's cooldown has expired and, if so, resets it
    /// </summary>
    /// <returns>Whether or not the cooldown has expired</returns>
    public bool CanPlay()
    {
        float currentTime = Time.time;
        return currentTime - lastPlayedTime >= soundCooldown;
    }

    /// <summary>
    /// Restarts this sound's cooldown
    /// </summary>
    public void ResetCooldown()
    {
        lastPlayedTime = Time.time;
    }
    
}
