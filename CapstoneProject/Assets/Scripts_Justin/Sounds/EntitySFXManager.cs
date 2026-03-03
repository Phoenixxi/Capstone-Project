using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;
/// <summary>
/// Script that stores the sounds of different entity actions or events and then tells the AudioManager to play them
/// </summary>
public class EntitySFXManager : MonoBehaviour
{
    [Header("Sounds")]
    //[SerializeField] private Sound jumpSound;
    //[SerializeField] private Sound hurtSound;
    //[SerializeField] private Sound deathSound;
    //[SerializeField] private Sound zoomBoomReactionSound;
    //[SerializeField] private Sound zoomGloomReactionSound;
    //[SerializeField] private Sound boomGloomReactionSound;
    //[SerializeField] private Sound zoomAttackSound;
    //[SerializeField] private Sound boomAttackSound;
    //[SerializeField] private Sound gloomAttackSound;
    //[SerializeField] private Sound normalAttackSound;
    [SerializeField] private SoundName jumpSound;
    [SerializeField] private SoundName hurtSound;
    [SerializeField] private SoundName deathSound;
    [SerializeField] private SoundName attackSound;


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
        entity.OnEntityKilledEvent += PlayDeathSound;
        entity.OnElementReactionEvent += PlayElementReactionSound;
        entity.OnEntityAttackEvent += PlayAttackSound;
    }

    private void OnDisable()
    {
        entity.OnJumpEvent -= PlayJumpSound;
        entity.OnEntityHurtEvent -= PlayHurtSound;
        entity.OnEntityKilledEvent -= PlayDeathSound;
        entity.OnEntityAttackEvent -= PlayAttackSound;
    }

    private void PlayJumpSound()
    {
        manager.PlaySound(jumpSound);
    }

    private void PlayHurtSound()
    {
        manager.PlaySound(hurtSound);
    }

    private void PlayDeathSound()
    {
        manager.PlaySound(deathSound);
    }

    private void PlayElementReactionSound(int reactionNum)
    {
        switch (reactionNum)
        {
            //ZOOM X BOOM
            case 1:
                manager.PlaySound(SoundName.ZOOM_BOOM_REACT);
                break;
            //ZOOM X GLOOM
            case 2:
                manager.PlaySound(SoundName.ZOOM_GLOOM_REACT);
                break;
            //BOOM X GLOOM
            case 3:
                manager.PlaySound(SoundName.BOOM_GLOOM_REACT);
                break;
        }
    }

    private void PlayAttackSound()
    {
        //    switch (element)
        //    {
        //        case ElementType.Zoom:
        //            //manager.PlaySoundRandom(zoomAttackSound);
        //            manager.PlaySound(zoomAttackSound);
        //            break;
        //        case ElementType.Boom:
        //            //manager.PlaySoundRandom(boomAttackSound);
        //            manager.PlaySound(boomAttackSound);
        //            break;
        //        case ElementType.Gloom:
        //            //manager.PlaySoundRandom(gloomAttackSound);
        //            manager.PlaySound(gloomAttackSound);
        //            break;
        //        default:
        //            //manager.PlaySoundRandom(normalAttackSound);
        //            manager.PlaySound(normalAttackSound);
        //            Debug.Log("Attempting to play attack sound");
        //            break;
        //    }
        //}
        manager.PlaySound(attackSound);
    }
}
