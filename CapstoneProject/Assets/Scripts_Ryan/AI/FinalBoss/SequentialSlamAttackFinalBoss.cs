using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialSlamAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private List<TentacleAttack> ListOfTentacleSlams;
    [SerializeField] private float timeBetweenSlams = 0.5f;
    private bool isAttacking = false;
    public override void Attack(Transform PlayerTransform)
    {
        isAttacking = true;
        StartCoroutine(SequentialAttack());
    }

    private IEnumerator SequentialAttack()
    {
        for(int i = 0; i < ListOfTentacleSlams.Count; i++)
        {
            ListOfTentacleSlams[i].Attack();
            yield return new WaitForSeconds(timeBetweenSlams);
        }

        UpdateTimeSinceLastAttack();
        isAttacking = false;
    }

    public override float GetDynamicWeight()
    {
        if(!HasCooldownExpired()) return 0f;
        return Mathf.Infinity;
    }

    public override bool IsAttacking()
    {
        return isAttacking;
    }
}
