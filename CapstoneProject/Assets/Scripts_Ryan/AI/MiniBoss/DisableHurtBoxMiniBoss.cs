using UnityEngine;

public class DisableHurtBoxMiniBoss : MonoBehaviour
{
    [SerializeField] private ExplodeHurtBox hurtbox;

    public void DisableHurtbox()
    {
        hurtbox.enabled = false;
    }
}
