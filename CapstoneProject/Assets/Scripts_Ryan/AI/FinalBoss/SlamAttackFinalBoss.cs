using System;
using UnityEditor;
using UnityEngine;

public class SlamAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private float range = 3f;
    [SerializeField] private Transform prefab;
    [SerializeField] private Transform Platform;
    [SerializeField] private int minSlams = 1;
    [SerializeField] private int maxSlams = 4;

    protected override void Awake()
    {
        base.Awake();
        finalBossController.PhaseTwo += SlamAttackPhaseTwo;
    }
    public override void Attack(Transform PlayerTransform)
    {
        if(!HasCooldownExpired()) return;

        int amountOfSlams = UnityEngine.Random.Range(minSlams, maxSlams);

        for(int i = 0; i < amountOfSlams; i++)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * range; //random point in circle
            Vector3 spawnPosition = new Vector3(PlayerTransform.position.x + randomPoint.x, Platform.position.y, PlayerTransform.position.z + randomPoint.y);

            Vector3 direction = -(PlayerTransform.position - spawnPosition).normalized;
            direction.y = 0f;

            if(direction == Vector3.zero)
            {
                direction = Vector3.forward;
            }

            Quaternion rotation = Quaternion.LookRotation(direction);

            Instantiate(prefab, spawnPosition, rotation);
        }

        UpdateTimeSinceLastAttack();
    }

    private void SlamAttackPhaseTwo()
    {
        int midSlams = (maxSlams + minSlams)/2;
        minSlams = midSlams;
    }
}
