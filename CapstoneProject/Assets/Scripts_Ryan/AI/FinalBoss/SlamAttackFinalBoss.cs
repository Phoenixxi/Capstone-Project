using UnityEditor;
using UnityEngine;

public class SlamAttackFinalBoss : FinalBossAttacks
{
    [SerializeField] private float range;
    [SerializeField] private Transform prefab;
    [SerializeField] private Transform Platform;
    public override void Attack(Transform PlayerTransform)
    {
        Vector2 randomPoint = Random.insideUnitCircle * range; //random point in circle
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

    protected override void AddToAttackQueue()
    {
        if(finalBossController == null) return;
        finalBossController.AddToAttackQueue(this);
    }
}
