using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MobSpawner : MonoBehaviour
{
    [SerializeField] private Transform ProjectileEnemyPrefab;
    [SerializeField] private Transform MeleeEnemyPrefab;
    [SerializeField] private int ProjectileCount;
    [SerializeField] private int MeleeCount;
    [SerializeField] private int range;

    public List<EntityManager> SpawnEnemies()
    {
        List<EntityManager> enemies = new List<EntityManager>();
        for(int i = 0; i < ProjectileCount; i++)
        {
            Vector3 pos;
            RandomPoint(transform.position, range, out pos);
            Transform enemy = Instantiate(ProjectileEnemyPrefab, pos, Quaternion.identity);
            EntityManager entityManager = enemy.GetComponentInChildren<EntityManager>();
            enemies.Add(entityManager);
        }

        for(int i = 0; i < MeleeCount; i++)
        {
            Vector3 pos;
            RandomPoint(transform.position, range, out pos);
            Transform enemy = Instantiate(MeleeEnemyPrefab, pos, Quaternion.identity);
            EntityManager entityManager = enemy.GetComponentInChildren<EntityManager>();
            enemies.Add(entityManager);
        }

        return enemies;
    }

    /// <summary>
    /// Picks a random point for the AI to go to
    /// </summary>
    /// <param name="center">Center of the Circle (Can be something like player.position)</param>
    /// <param name="range">How far can the enemy AI go within the circle (Radius of the circle)</param>
    /// <param name="result">The point to where the enemy AI will go to</param>
    /// <returns></returns>
    private void RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector2 vector2 = Random.insideUnitCircle * range;
        Vector3 randomPoint = center + new Vector3(vector2.x, 0f, vector2.y);
        NavMeshHit hit;

        //Navmesh.SamplePosition finds the nearest navigable (walkable) point on the NavMesh
        //within a given radius
        if (NavMesh.SamplePosition(randomPoint, out hit, range, NavMesh.AllAreas))
        {
            //if there is a point on the navmesh near randomPoint
            //that the AI can walk to, return that result
            result = hit.position;
            return;
        }

        result = center;
        return;
    }
}
