using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MeatballHazard : MonoBehaviour
{
    [Header("💥 VFX & Visuals")]
    public GameObject explosionVFX;
    public GameObject visualModel;
    public GameObject indicator;

    [Header("📏 Prediction Settings")]
    public float triggerDistance = 0.3f;
    public float castRadius = 0.3f;

    [Header("⏱️ Timing & Damage Settings")]
    public float damageDelay = 0.05f;
    public float destroyDelay = 2.0f;
    public float damageRadius = 5f;
    public float damageHeight = 3f;
    public LayerMask damageLayer;
    public string targetTag = "Player";

    [Header("🚀 Physics")]
    public float maxSpeed = 25f;
    [SerializeField] private float damage = 10f;

    [Header("🛠️ Debug & Cleanup")]
    [Tooltip("Maximum life time in seconds if it doesn't hit anything (fallback).")]
    public float maxLifeTime = 10f; 

    private Rigidbody rb;
    private bool isExploding = false;
    private Vector3 indicatorPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // --- The Fallback Timer ---
        // If the meatball falls out of the world or gets stuck, it will self-destruct.
        Destroy(gameObject, maxLifeTime);
        // --------------------------

        if (explosionVFX != null) explosionVFX.SetActive(false);
        indicator.SetActive(true);
        RaycastHit landingPosition;
        if(Physics.SphereCast(origin: transform.position, radius: castRadius, direction: Vector3.down, out landingPosition, maxLifeTime * maxSpeed, ~0, QueryTriggerInteraction.Ignore))
        {
            indicatorPosition = landingPosition.point;
        } else
        {
            indicatorPosition = transform.position - new Vector3(0, maxLifeTime * maxSpeed, 0);
        }
    }

    void FixedUpdate()
    {
        if (isExploding) return;

        if (rb != null && rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        PredictAnyImpact();
    }

    private void Update()
    {
        indicator.transform.position = indicatorPosition;
    }

    private void PredictAnyImpact()
    {
        float currentVelocity = rb.linearVelocity.magnitude;
        float castDistance = (currentVelocity * Time.fixedDeltaTime) + triggerDistance;

        RaycastHit hit;
        if (Physics.SphereCast(transform.position, castRadius, Vector3.down, out hit, castDistance))
        {
            if (hit.collider.gameObject != gameObject && !hit.collider.isTrigger)
            {
                StartExplosionSequence();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isExploding)
        {
            StartExplosionSequence();
        }
    }

    private void StartExplosionSequence()
    {
        isExploding = true;

        rb.isKinematic = true;
        rb.linearVelocity = Vector3.zero;

        if (visualModel != null) visualModel.SetActive(false);

        if (explosionVFX != null)
        {
            explosionVFX.transform.SetParent(null);
            explosionVFX.transform.position = transform.position;
            explosionVFX.SetActive(true);

            ParticleSystem ps = explosionVFX.GetComponent<ParticleSystem>();
            if (ps == null) ps = explosionVFX.GetComponentInChildren<ParticleSystem>();
            if (ps != null) ps.Play();
        }

        StartCoroutine(ExplosionRoutine());
    }

    private IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(damageDelay);
        DealDamage();

        yield return new WaitForSeconds(destroyDelay);

        if (explosionVFX != null) Destroy(explosionVFX);
        Destroy(gameObject);
    }

    private void DealDamage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius, damageLayer);
        foreach (Collider hit in hitColliders)
        {
            if (hit.gameObject.CompareTag(targetTag))
            {
                EntityManager em = hit.gameObject.GetComponentInChildren<EntityManager>();
                if (em != null) em.TakeDamage(damage);
            }
        }
    }
}