using UnityEngine;

public class GloomVFXController : MonoBehaviour
{
    private ParticleSystem ps;
    [SerializeField] private float duration;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        Invoke(nameof(DestroyVFX), duration);
    }

    void DestroyVFX()
    {
        Destroy(gameObject);
    }
}
