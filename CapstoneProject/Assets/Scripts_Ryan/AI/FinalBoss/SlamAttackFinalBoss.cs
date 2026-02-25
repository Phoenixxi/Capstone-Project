using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SlamAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private List<TentacleAttack> ListOfTentacleSlams;
    [SerializeField] private int minSlams = 1;
    [SerializeField] private int maxSlams = 4;
    [SerializeField] private float SequentialAttackChance = 50f;
    [SerializeField] private float timeBetweenSlams = 0.5f;
    [SerializeField] protected float maxRecoveryTime = 3f;
    [SerializeField] private AnimationCurve weightCurve;

    private bool SequentialAttack = false;

    protected override void Awake()
    {
        base.Awake();
        finalBossController.PhaseTwo += SlamAttackPhaseTwo;
    }
    public override void Attack(Transform PlayerTransform)
    {
        if(!HasCooldownExpired()) return;

        if(UnityEngine.Random.Range(0, 1) <= SequentialAttackChance/100f)
        {
            SequentialAttack = true;
        }
        else
        {
            SequentialAttack = false;
        }

        int amountOfSlams = UnityEngine.Random.Range(minSlams, maxSlams);

        PriorityQueue<TentacleAttack> TentaclesToAttack = new PriorityQueue<TentacleAttack>();

        for(int i = 0; i < ListOfTentacleSlams.Count; i++)
        {
            TentacleAttack tentacleAttack = ListOfTentacleSlams[i];
            float distanceToPlayer = Vector3.Distance(tentacleAttack.transform.position, PlayerTransform.position);
            TentaclesToAttack.Enqueue(tentacleAttack, -distanceToPlayer);
        }

        if(SequentialAttack)
        {
            StartCoroutine(SequentialAttacks(TentaclesToAttack, amountOfSlams));
        }
        else
        {
            SlamAttackNormal(TentaclesToAttack, amountOfSlams);
        }
    }

    private IEnumerator SequentialAttacks(PriorityQueue<TentacleAttack> TentaclesToAttack, int amountOfSlams)
    {
        for(int i = 0; i < amountOfSlams; i++)
        {
            TentacleAttack tentacleAttack = TentaclesToAttack.Pop();
            tentacleAttack.Attack();
            yield return new WaitForSeconds(timeBetweenSlams);
        }

        UpdateTimeSinceLastAttack();
    }

    private void SlamAttackNormal(PriorityQueue<TentacleAttack> TentaclesToAttack, int amountOfSlams)
    {
        for(int i = 0; i < amountOfSlams; i++)
        {
            TentacleAttack tentacleAttack = TentaclesToAttack.Pop();
            tentacleAttack.Attack();
        }

        UpdateTimeSinceLastAttack();
    }

    private void SlamAttackPhaseTwo()
    {
        int midSlams = (maxSlams + minSlams)/2;
        minSlams = midSlams;
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
