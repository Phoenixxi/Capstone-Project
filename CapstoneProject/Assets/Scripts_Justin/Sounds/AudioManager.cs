using System;
using System.Collections;
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
    [SerializeField] private float songTransitionTime = 0.25f;

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
                //sources[(int)queuedSound.type].volume = queuedSound.volume;
                //sources[(int)queuedSound.type].clip = queuedSound.audioClip;
                //sources[(int)queuedSound.type].Play();
                int currentMusicSource = (int)queuedSound.type;
                if (sources[currentMusicSource].isPlaying) StartCoroutine(MusicTransitionCoroutine(currentMusicSource, currentMusicSource + 2, queuedSound));
                else StartCoroutine(MusicTransitionCoroutine(currentMusicSource + 2, currentMusicSource, queuedSound));
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

    /// <summary>
    /// Coroutine that gradually transitions from one song to another
    /// </summary>
    /// <param name="currentSourceIndex">The index of the current music source</param>
    /// <param name="nextSourceIndex">The index of the new music source</param>
    /// <returns></returns>
    private IEnumerator MusicTransitionCoroutine(int currentSourceIndex, int nextSourceIndex, SoundEffect song)
    {
        AudioSource currentSource = sources[currentSourceIndex];
        AudioSource nextSource = sources[nextSourceIndex];
        nextSource.volume = 0f;
        nextSource.clip = song.audioClip;
        float currentTransitionTime = 0f;
        float currentSongVolume = currentSource.volume;
        nextSource.Play();
        while(currentTransitionTime < songTransitionTime)
        {
            currentTransitionTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentSongVolume, 0f, currentTransitionTime / songTransitionTime);
            currentSource.volume = newVolume;
            newVolume = Mathf.Lerp(0f, song.volume, currentTransitionTime / songTransitionTime);
            nextSource.volume = newVolume;
            yield return null;
        }
        currentSource.Stop();
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
    JUMP = 0,
    PLAYER_HURT,
    PLAYER_DEATH,
    ZOOM_ATTACK,
    BOOM_ATTACK,
    GLOOM_ATTACK,
    ZOOM_DASH,
    BOOM_GROUND_POUND,
    GLOOM_AURA,
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
    BOSS_THEME_PHASE_2,
    BOOM_BOMB_EXPLOSION,
    BUTTON_CLICK
}
