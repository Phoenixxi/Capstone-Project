using UnityEngine;

public class EnemyControllerMelee : EnemyController
{
    [Header("Amount of seconds the enemy will stay still after attacking \nDefault is 1")]
    [SerializeField] private float DelaySeconds;

    protected override void Awake()
    {
        if(DelaySeconds == 0f)
        {
            DelaySeconds = 1f;
        }

        base.Awake();
    }

    protected override void initializeStateDictionary()
    {
        stateDic.Add(AIStateType.Combat, new CombatStateMelee());
        stateDic.Add(AIStateType.Chasing, new ChasingStateMelee());
        stateDic.Add(AIStateType.Wandering, new WanderingStateMelee());
        stateDic.Add(AIStateType.Delay, new DelayStateMelee(DelaySeconds));
    }
}
