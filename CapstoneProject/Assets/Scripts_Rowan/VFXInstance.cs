using UnityEngine;

public class VFXInstance : MonoBehaviour
{
    private ParticleSystem ps;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        if (!ps.main.playOnAwake) ps.Play();
    }

    void Update()
    {
        if (ps != null && !ps.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
