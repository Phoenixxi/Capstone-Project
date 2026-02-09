using UnityEngine;

public class EnemyControllerExplode : EnemyController
{
    protected override void initializeStateDictionary()
    {
        stateDic.Add(AIStateType.Combat, new CombatStateExplode());
        stateDic.Add(AIStateType.Chasing, new ChasingStateExplode());
        stateDic.Add(AIStateType.Wandering, new WanderingStateExplode());
        stateDic.Add(AIStateType.Delay, new DelayStateMelee(1000));
    }
}
