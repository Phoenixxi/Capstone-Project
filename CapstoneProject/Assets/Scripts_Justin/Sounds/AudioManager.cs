using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script responsible for playing audio throughout the game
/// </summary>
[ExecuteAlways]
public class AudioManager : MonoBehaviour
{
    /*
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
    */

    /// <summary>
    /// Helper class that stores SFX information
    /// </summary>
    [Serializable]
    private class SoundEffect
    {
        public enum SoundType
        {
            SFX = 0,
            MUSIC = 1
        }
        
        [HideInInspector] public string name;
        public float volume = 1f;
        public AudioClip audioClip;
        public SoundType type;
        public bool variedPitch = false;
        [Range(-3, 3)] public float pitchUpperBound = 1f;
        [Range(-3, 3)] public float pitchLowerBound = 1f;
    }

    //==========================
    //COMPONENT CODE STARTS HERE
    //==========================
    [Header("IMPORTANT: DON'T CHANGE AUDIO SOURCE ORDER")]
    [SerializeField] private AudioSource[] sources; //IMPORTANT: The order that the audio sources are assigned is important, so unless you know what you're doing, DON'T MESS WITH THIS
    [SerializeField] private SoundEffect[] sounds;

    //Queue and set used to prevent overlapping sounds
    private HashSet<SoundName> usedSounds;
    private Queue<SoundName> soundQueue;

    public void PlaySound(SoundName soundName)
    {
        if (!usedSounds.Contains(soundName))
        {
            usedSounds.Add(soundName);
            soundQueue.Enqueue(soundName);
        }
    }

    private void Update()
    {
        while(soundQueue != null && soundQueue.Count > 0)
        {
            SoundEffect queuedSound = sounds[(int)soundQueue.Dequeue()];
            if (queuedSound.type == SoundEffect.SoundType.MUSIC)
            {
                sources[(int)queuedSound.type].volume = queuedSound.volume;
                sources[(int)queuedSound.type].clip = queuedSound.audioClip;
                sources[(int)queuedSound.type].Play();
            }
            else if (!queuedSound.variedPitch)
            {
                sources[(int)queuedSound.type].PlayOneShot(queuedSound.audioClip, queuedSound.volume);
            }
            else
            {
                sources[2].pitch = UnityEngine.Random.Range(queuedSound.pitchLowerBound, queuedSound.pitchUpperBound);
                sources[2].PlayOneShot(queuedSound.audioClip, queuedSound.volume);
            }
        }
        usedSounds.Clear();
    }

    //This makes it more convenient to modify the different sounds in the editor
#if UNITY_EDITOR
    private void OnEnable()
    {
        usedSounds = new HashSet<SoundName>();
        soundQueue = new Queue<SoundName>();
        string[] soundNames = Enum.GetNames(typeof(SoundName));
        Array.Resize(ref sounds, soundNames.Length);
        for (int i = 0; i < soundNames.Length; i++)
        {
            sounds[i].name = soundNames[i];
        }
    }
#endif

}

/// <summary>
/// List of the names of every sound effect in the game
/// </summary>
public enum SoundName
{
    JUMP,
    PLAYER_HURT,
    PLAYER_DEATH,
    ZOOM_ATTACK,
    BOOM_ATTACK,
    GLOOM_ATTACK,
    ENEMY_HURT,
    ENEMY_DEATH,
    ENEMY_ATTACK,
    ZOOM_BOOM_REACT,
    ZOOM_GLOOM_REACT,
    BOOM_GLOOM_REACT,
    TOOTH_ATTACK,
    TENTACLE_WINDUP,
    TENTACLE_SLAM,
    TENTACLE_SWIPE,
    BOSS_THEME,
    BOSS_THEME_PHASE_2
}
