using UnityEngine;

/// <summary>
/// Simple class for triggering a sound when the player touches a trigger
/// </summary>
public class SoundTrigger : MonoBehaviour
{
    [SerializeField] private SoundName soundName;
    private AudioManager audioManager;
    private bool enabled;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioManager = FindFirstObjectByType<AudioManager>();
        enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && enabled)
        {
            audioManager.PlaySound(soundName);
            enabled = false;
        }
    }
}
