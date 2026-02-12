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
    private Vector3 topMidPoint;
    private Vector3 bottomMidPoint;
    protected override void Awake()
    {
        topMidPoint = Vector3.Lerp(topRightSwipe.position, topLeftSwipe.position, 0.5f);
        bottomMidPoint = Vector3.Lerp(bottomRightSwipe.position, bottomLeftSwipe.position, 0.5f);

        base.Awake();
    }
    public override void Attack(Transform PlayerTransform)
    {
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
    }

    protected override void AddToAttackQueue()
    {
        if(finalBossController == null) return;
        finalBossController.AddToAttackQueue(this);
    }
}
