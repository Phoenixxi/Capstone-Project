using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent; //The navmeshagent component that handles pathfinding
    private GameObject player; //The player object that the enemy AI will path towards
    void Awake()
    {
        player = GameObject.Find("Player"); //Find the player in the world
        navMeshAgent = GetComponent<NavMeshAgent>(); //Get the navmeshagent component from this enemy
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.SetDestination(player.transform.position); //set its path towards the player. This will automatically move enemy towards player
    }

    /// <summary>
    /// Change the speed at which the enemy moves at
    /// </summary>
    /// <param name="newVelocity"> The new speed the enemy should move at </param>
    public void ChangeVelocity(float newVelocity)
    {
        navMeshAgent.speed = newVelocity;
    }
}
