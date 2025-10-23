using TMPro;
using UnityEngine;
using UnityEngine.AI;
using ElementType = lilGuysNamespace.EntityData.ElementType;

public class AIContext
{
    //Fields that we shouldn't have to update in states
    public NavMeshAgent agent { get; private set; }
    public Transform EnemyTransform { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public EntityManager entityManagerEnemy { get; private set; }
    public EntityManager entityManagerPlayer { get; private set; }
    public ElementType EnemyType { get; private set; }
    public float AttackRange { get; private set; }
    public float LineOfSightRange { get; private set; }

    //fields that we need to update after a state is complete
    public float DistanceToPlayer => Vector3.Distance(EnemyTransform.position, PlayerTransform.position);
    public ElementType PlayerType => entityManagerPlayer.defaultElement;
    public bool isGrounded;

    public AIContext(NavMeshAgent navMeshAgent, Transform eTransform, Transform pTransform, float range, float LineOfSightRange)
    {
        agent = navMeshAgent;
        EnemyTransform = eTransform;
        PlayerTransform = pTransform;

        entityManagerEnemy = EnemyTransform.GetComponent<EntityManager>();
        entityManagerPlayer = PlayerTransform.GetComponent<EntityManager>();

        EnemyType = entityManagerEnemy.defaultElement;

        AttackRange = range;
        this.LineOfSightRange = LineOfSightRange;
    }
}
