using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Script responsible for playing audio throughout the game
/// </summary>
public class AudioManager : MonoBehaviour
{
    /// <summary>
    /// Helper class that stores SFX information
    /// </summary>
    [Serializable]
    public class SoundEffect
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
        public float cooldown = 0f;
        [HideInInspector] public float lastUsedTime = 0f;
    }

    //==========================
    //COMPONENT CODE STARTS HERE
    //==========================
    [Header("IMPORTANT: DON'T CHANGE AUDIO SOURCE ORDER")]
    [SerializeField] private AudioSource[] sources; //IMPORTANT: The order that the audio sources are assigned is important, so unless you know what you're doing, DON'T MESS WITH THIS
    //[SerializeField] private SoundEffect[] sounds;
    [SerializeField] private SoundLibrary sounds;
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

    private void Awake()
    {
        soundQueue = new Queue<SoundName>();
        usedSounds = new HashSet<SoundName>();
    }

    private void Update()
    {
        while(soundQueue != null && soundQueue.Count > 0)
        {
            SoundEffect queuedSound = sounds.soundList[(int)soundQueue.Dequeue()];
            if (Time.time - queuedSound.lastUsedTime < queuedSound.cooldown) continue;
            queuedSound.lastUsedTime = Time.time;
            if (queuedSound.type == SoundEffect.SoundType.MUSIC)
            {
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
    /// Causes the current background music to be temporarily dampened for a duration
    /// </summary>
    /// <param name="duration">How long the music should be dampened for</param>
    public void DampenMusic(float duration)
    {
        int currentSourceIndex = (sources[(int)SoundEffect.SoundType.MUSIC].isPlaying) ? (int)SoundEffect.SoundType.MUSIC : (int)SoundEffect.SoundType.MUSIC + 2;
        StartCoroutine(MusicDampenCoroutine(currentSourceIndex, duration));
    }

    private IEnumerator MusicDampenCoroutine(int currentSourceIndex, float dampenDuration)
    {
        AudioSource currentSource = sources[currentSourceIndex];
        float currentTransitionTime = 0f;
        float currentSongVolume = currentSource.volume;
        while(currentTransitionTime < songTransitionTime)
        {
            currentTransitionTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(currentSongVolume, 0f, currentTransitionTime / songTransitionTime);
            currentSource.volume = newVolume;
            yield return null;
        }
        yield return new WaitForSeconds(dampenDuration);
        currentTransitionTime = 0f;
        while(currentTransitionTime < songTransitionTime)
        {
            currentTransitionTime += Time.deltaTime;
            float newVolume = Mathf.Lerp(0f, currentSongVolume, currentTransitionTime / songTransitionTime);
            currentSource.volume = newVolume;
            yield return null;
        }
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
    BUTTON_CLICK,
    BOSS_HURT,
    MINIBOSS_THEME,
    COLOSSEUM_THENE,
    TOOTH_ATTACK_WARNING,
    STAGE_HEAl
}