using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script responsible for playing audio throughout the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource staticAudioSource;
    [SerializeField] private AudioSource randomAudioSource;
    [SerializeField]
    [Range(-3, 3)]private float randomPitchLowerBound = 1f;
    [SerializeField] 
    [Range(-3, 3)] private float randomPitchUpperBound = 1f;
    private Queue<Sound> soundQueue = new Queue<Sound>();
    private HashSet<AudioClip> usedSounds = new HashSet<AudioClip>();

    /// <summary>
    /// Plays a given sound at the given volume
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="volume">The volume to play the sound at. Should be between 0 and 1</param>
    public void PlaySound(Sound sound)
    {
        if (!sound.CanPlay()) return;
        sound.ResetCooldown();
        //staticAudioSource.PlayOneShot(sound.SoundClip, sound.Volume);
        soundQueue.Enqueue(sound);
    }

    /// <summary>
    /// Plays a given sound at the given volume with a randomized pitch
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="volume">The volume to play the sound at. Should be between 0 and 1</param>
    //public void PlaySoundRandom(Sound sound)
    //{
    //    if (!sound.CanPlay()) return;
    //    sound.ResetCooldown();
    //    randomAudioSource.pitch = Random.Range(randomPitchLowerBound, randomPitchUpperBound);
    //    randomAudioSource.PlayOneShot(sound.SoundClip, sound.Volume);
    //}

    private void Update()
    {
        while(soundQueue.Count > 0)
        {
            Sound sound = soundQueue.Dequeue();
            if (usedSounds.Contains(sound.SoundClip)) continue;
            usedSounds.Add(sound.SoundClip);
            if(sound.HasRandomPitch)
            {
                randomAudioSource.pitch = Random.Range(randomPitchLowerBound, randomPitchUpperBound);
                randomAudioSource.PlayOneShot(sound.SoundClip, sound.Volume);
            }
            else
            {
                staticAudioSource.PlayOneShot(sound.SoundClip, sound.Volume);
            }
        }
        usedSounds.Clear();
    }
}
