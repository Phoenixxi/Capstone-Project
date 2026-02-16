using UnityEngine;

public enum Priority{None,First,Second,Third}
public abstract class FinalBossAttacks : MonoBehaviour 
{
    [SerializeField] protected float CoolDownDuration;
    [SerializeField] protected FinalBossController finalBossController;
    public Priority priority;

    private float timeSinceLastAddedToQueue = 0f;

    protected virtual void Awake()
    {
        finalBossController = GetComponent<FinalBossController>();
        finalBossController.PhaseTwo += CutCoolDownDuration;
    }

    protected virtual void Start()
    {
        
    }

    void Update()
    {
        if(HasCooldownExpired())
        {
            AddToAttackQueue();
            timeSinceLastAddedToQueue = Time.time;
        }
    }

    private bool HasCooldownExpired()
    {
        float currentTime = Time.time;
        return currentTime - timeSinceLastAddedToQueue >= CoolDownDuration;
    }

    private void CutCoolDownDuration()
    {
        CoolDownDuration -= CoolDownDuration * (finalBossController.DecreaseAttackCoolDownPercentage/100f);
    }

    protected abstract void AddToAttackQueue();

    public abstract void Attack(Transform PlayerTransform);
}
