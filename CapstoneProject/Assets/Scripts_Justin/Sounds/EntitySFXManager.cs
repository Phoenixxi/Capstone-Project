using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;
/// <summary>
/// Script that stores the sounds of different entity actions or events and then tells the AudioManager to play them
/// </summary>
public class EntitySFXManager : MonoBehaviour
{
    [Header("Sounds")]
    [SerializeField] private Sound jumpSound;
    [SerializeField] private Sound hurtSound;
    [SerializeField] private Sound deathSound;
    [SerializeField] private Sound zoomBoomReactionSound;
    [SerializeField] private Sound zoomGloomReactionSound;
    [SerializeField] private Sound boomGloomReactionSound;
    [SerializeField] private Sound zoomAttackSound;
    [SerializeField] private Sound boomAttackSound;
    [SerializeField] private Sound gloomAttackSound;
    

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
    }

    private void OnDisable()
    {
        entity.OnJumpEvent -= PlayJumpSound;
        entity.OnEntityHurtEvent -= PlayHurtSound;
        entity.OnEntityKilledEvent -= PlayDeathSound;
    }

    private void PlayJumpSound()
    {
        manager.PlaySound(jumpSound);
    }

    private void PlayHurtSound(ElementType element)
    {
        manager.PlaySoundRandom(hurtSound);
        //switch (element)
        //{
        //    case ElementType.Zoom:
        //        manager.PlaySoundRandom(zoomHitSound);
        //        break;
        //    case ElementType.Boom:
        //        manager.PlaySoundRandom(boomHitSound);
        //        break;
        //    case ElementType.Gloom:
        //        manager.PlaySoundRandom(gloomHitSound);
        //        break;
        //}
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
                manager.PlaySound(zoomBoomReactionSound);
                break;
            //ZOOM X GLOOM
            case 2:
                manager.PlaySound(zoomGloomReactionSound);
                break;
            //BOOM X GLOOM
            case 3:
                manager.PlaySound(boomGloomReactionSound);
                break;
        }
    }
}
