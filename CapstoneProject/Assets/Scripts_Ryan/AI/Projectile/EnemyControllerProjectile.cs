using Unity.VisualScripting;
using UnityEngine;

public class EnemyControllerProjectile : EnemyController
{
    protected override void initializeStateDictionary()
    {
        stateDic.Add(AIStateType.Combat, new CombatStateProjectile());
        stateDic.Add(AIStateType.Chasing, new ChasingStateProjectile());
        stateDic.Add(AIStateType.Wandering, new WanderingStateProjectile());
        stateDic.Add(AIStateType.Hover, new WanderingStateProjectile(3f, true));
    }
}
