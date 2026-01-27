using UnityEngine;

public class TutorialChaseEnemyController : EnemyController
{
    protected override void initializeStateDictionary()
    {
        stateDic.Add(AIStateType.Chasing, new ChaseStateTutorialChasingEnemy());
        stateDic.Add(AIStateType.Wandering, new WanderStateTutorialChasingEnemy(0f, false));
    }
}
