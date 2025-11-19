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

    /// <summary>
    /// Plays a given sound at the given volume
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="volume">The volume to play the sound at. Should be between 0 and 1</param>
    public void PlaySound(AudioClip sound, float volume)
    {
        staticAudioSource.PlayOneShot(sound, volume);
    }

    /// <summary>
    /// Plays a given sound at the given volume with a randomized pitch
    /// </summary>
    /// <param name="sound">The sound to play</param>
    /// <param name="volume">The volume to play the sound at. Should be between 0 and 1</param>
    public void PlaySoundRandom(AudioClip sound, float volume)
    {
        randomAudioSource.pitch = Random.Range(randomPitchLowerBound, randomPitchUpperBound);
        randomAudioSource.PlayOneShot(sound, volume);
    }
}
