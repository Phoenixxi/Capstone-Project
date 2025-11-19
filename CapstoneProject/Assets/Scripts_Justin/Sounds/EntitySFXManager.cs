using UnityEngine;
/// <summary>
/// Script that stores the sounds of different entity actions or events and then tells the AudioManager to play them
/// </summary>
public class EntitySFXManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;
    [SerializeField]
    [Range(0, 1)] private float jumpVolume;

    [SerializeField] private AudioClip hurtSound;
    [SerializeField]
    [Range(0, 1)] private float hurtVolume;

    private EntityManager entity;
    private AudioManager manager;


    private void Awake()
    {
        entity = GetComponent<EntityManager>();
        manager = FindFirstObjectByType<AudioManager>();
    }

    private void OnEnable()
    {
        entity.OnJumpEvent += PlayJumpSound;
        entity.OnEntityHurtEvent += PlayHurtSound;
    }

    private void OnDisable()
    {
        entity.OnJumpEvent -= PlayJumpSound;
        entity.OnEntityHurtEvent -= PlayHurtSound;
    }

    private void PlayJumpSound()
    {
        manager.PlaySound(jumpSound, jumpVolume);
    }

    private void PlayHurtSound()
    {
        manager.PlaySoundRandom(hurtSound, hurtVolume);
    }
}
