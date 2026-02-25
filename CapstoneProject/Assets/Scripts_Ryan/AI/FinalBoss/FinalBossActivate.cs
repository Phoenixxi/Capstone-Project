using UnityEngine;

public class FinalBossActivate : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private Collider blockCollider;

    void Awake()
    {
        triggerCollider.isTrigger = true;
        blockCollider.isTrigger = false;
        blockCollider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            blockCollider.enabled = true;
            FinalBossManagerSingleton.Instance.InitializeBoss();
            Destroy(triggerCollider);
        }
    }
}
