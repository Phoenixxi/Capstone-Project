using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class SwipeAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private Transform topRightSwipe;
    [SerializeField] private Transform topLeftSwipe;
    [SerializeField] private Transform bottomRightSwipe;
    [SerializeField] private Transform bottomLeftSwipe;
    [SerializeField] protected float maxRecoveryTime = 4f;
    [SerializeField] private AnimationCurve weightCurve;
    private Vector3 topMidPoint;
    private Vector3 bottomMidPoint;
    protected override void Awake()
    {
        topMidPoint = Vector3.Lerp(topRightSwipe.position, topLeftSwipe.position, 0.5f);
        bottomMidPoint = Vector3.Lerp(bottomRightSwipe.position, bottomLeftSwipe.position, 0.5f);

        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        topLeftSwipe.localScale = new Vector3(-1, 1, 1);
        bottomRightSwipe.localScale = new Vector3(-1, 1, 1);
    }
    public override void Attack(Transform PlayerTransform)
    {
        if(!HasCooldownExpired()) return;

        float topDistance = Vector3.Distance(PlayerTransform.position, topMidPoint);
        float bottomDistance = Vector3.Distance(PlayerTransform.position, bottomMidPoint);

        if(topDistance > bottomDistance)
        {
            bottomRightSwipe.GetComponent<TentacleAttack>().Attack();
            bottomLeftSwipe.GetComponent<TentacleAttack>().Attack();
        }
        else
        {
            topRightSwipe.GetComponent<TentacleAttack>().Attack();
            topLeftSwipe.GetComponent<TentacleAttack>().Attack();
        }

        UpdateTimeSinceLastAttack();
    }

    public override float GetDynamicWeight()
    {
        if(!HasCooldownExpired()) return 0f;

        float timeSince = Time.time - timeSinceLastAttacked;
        float weight = Mathf.Clamp01(timeSince / maxRecoveryTime);
        float dynamicWeight = weightCurve.Evaluate(weight);

        float maxRecoveryBonus = 10f;
        dynamicWeight *= maxRecoveryBonus;

        if(UnityEngine.Random.Range(0f, 1f) <= Chance/100f)
        {
            dynamicWeight += UnityEngine.Random.Range(0f, maxRecoveryBonus * 0.3f);
        }

        return dynamicWeight;
    }

    public override bool IsAttacking()
    {
        return isAttacking;
    }
}
