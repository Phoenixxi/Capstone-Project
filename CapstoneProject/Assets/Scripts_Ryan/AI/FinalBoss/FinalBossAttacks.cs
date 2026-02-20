using UnityEngine;

public abstract class FinalBossAttacks : MonoBehaviour 
{
    [SerializeField] protected float CoolDownDuration;
    [SerializeField] protected FinalBossController finalBossController;
    [SerializeField] protected float maxRecoveryTime;
    [SerializeField] protected float Chance;
    [SerializeField] protected AnimationCurve weightCurve;

    private float timeSinceLastAttacked;

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

    protected bool HasCooldownExpired()
    {
        float currentTime = Time.time;
        return currentTime - timeSinceLastAttacked >= CoolDownDuration;
    }

    private void CutCoolDownDuration()
    {
        CoolDownDuration -= CoolDownDuration * (finalBossController.DecreaseAttackCoolDownPercentage/100f);
    }

    public float GetDynamicWeight()
    {
        if(!HasCooldownExpired()) return 0f;

        float timeSince = Time.time - timeSinceLastAttacked;
        float weight = Mathf.Clamp01(timeSince / maxRecoveryTime);
        float dynamicWeight = weightCurve.Evaluate(weight);

        float maxRecoveryBonus = 10f;
        dynamicWeight *= maxRecoveryBonus;

        if(Random.Range(0f, 1f) <= Chance/100f)
        {
            dynamicWeight += Random.Range(0f, maxRecoveryBonus * 0.3f);
        }

        return dynamicWeight;
    }

    public abstract void Attack(Transform PlayerTransform);
}
