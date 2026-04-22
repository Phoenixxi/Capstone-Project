using UnityEngine;

public class GraveSoundPlayer : MonoBehaviour
{
    AudioManager audioManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        audioManager = FindFirstObjectByType<AudioManager>();   
    }

    public void PlayGraveSound()
    {
        audioManager.PlaySound(SoundName.GRAVE_DROP);
    }


}
