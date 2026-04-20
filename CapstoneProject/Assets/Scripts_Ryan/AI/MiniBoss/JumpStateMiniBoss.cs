
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;

public class JumpStateMiniBoss : IState
{
    private Vector3 target;
    private Vector3 jumpStart;
    private float jumpHeight = 5f;
    private float jumpDuration = 2f;
    private float elapsedTime = 0f;
    public AIStateType CheckTransition(AIContext aIContext)
    {
        return AIStateType.Delay;
    }

    public void OnEnter(AIContext aIContext)
    {
        NavMeshHit hit;

        if(NavMesh.SamplePosition(aIContext.PlayerTransform.position, out hit, 3.0f, NavMesh.AllAreas))
        {
            target = hit.position;
        }
        else
        {
            target = aIContext.EnemyTransform.position;
        }

        elapsedTime = 0f;
        jumpStart = aIContext.EnemyTransform.position;
        aIContext.animator.SetTrigger("Jump");
    }

    public void OnExit(AIContext aIContext)
    {
        
    }

    public AIStateType UpdateAI(AIContext aIContext)
    {
        // NavMeshAgent agent = aIContext.agent;

        // agent.isStopped = true;
        // agent.updatePosition = false;
        // agent.updateRotation = false;

        float t = elapsedTime/jumpDuration;

        Vector3 pos = Vector3.Lerp(jumpStart, target, t);

        float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
        pos.y += height;

        Vector3 delta = pos - aIContext.EnemyTransform.position;

        aIContext.characterController.Move(delta);

        elapsedTime += Time.deltaTime;

        if(t >= jumpDuration)
        {
            return CheckTransition(aIContext);
        }

        return AIStateType.Jump;
    }
}
