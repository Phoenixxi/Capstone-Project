using UnityEngine;

/// <summary>
/// Script responsible for playing sounds for a particular entity. Must be placed on a game object with an Audio Source and EntityManager component
/// </summary>
public class EntitySFXManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSrc;
    [Header("Sounds")]
    [SerializeField] private AudioClip jumpSound;



    private EntityManager entity;


    private void Awake()
    {
        entity = GetComponent<EntityManager>();
    }

    private void OnEnable()
    {
        entity.OnJumpEvent += PlayJumpSound;
    }

    private void OnDisable()
    {
        entity.OnJumpEvent -= PlayJumpSound;
    }

    private void PlayJumpSound()
    {
        audioSrc.PlayOneShot(jumpSound);
    }
}
