using UnityEngine;

public abstract class FinalBossAttacks : MonoBehaviour 
{
    [SerializeField] protected float CoolDownDuration;
    [SerializeField] protected FinalBossController finalBossController;
    [SerializeField] protected float Chance;

    protected float timeSinceLastAttacked;

    protected virtual void Awake()
    {
        timeSinceLastAttacked = CoolDownDuration;
        finalBossController = GetComponent<FinalBossController>();
        finalBossController.PhaseTwo += CutCoolDownDuration;
    }

    protected virtual void Start()
    {
        
    }

    protected void UpdateTimeSinceLastAttack()
    {
        timeSinceLastAttacked = Time.time;
    }

    public bool HasCooldownExpired()
    {
        float currentTime = Time.time;
        return currentTime - timeSinceLastAttacked >= CoolDownDuration;
    }

    private void CutCoolDownDuration()
    {
        CoolDownDuration -= CoolDownDuration * (finalBossController.DecreaseAttackCoolDownPercentage/100f);
    }

    public abstract bool IsAttacking();

    public abstract float GetDynamicWeight();

    public abstract void Attack(Transform PlayerTransform);
}
