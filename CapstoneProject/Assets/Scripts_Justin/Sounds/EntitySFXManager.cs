using UnityEngine;
/// <summary>
/// Script that stores the sounds of different entity actions or events and then tells the AudioManager to play them
/// </summary>
public class EntitySFXManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private Sound jumpSound;


    [SerializeField] private Sound hurtSound;

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
        manager.PlaySound(jumpSound);
    }

    private void PlayHurtSound()
    {
        manager.PlaySoundRandom(hurtSound);
    }
}
