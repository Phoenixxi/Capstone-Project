using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBlocker : MonoBehaviour
{
    [Header("List of mob spawners beyond this level blocker and before the next level blocker")]
    [SerializeField] private List<MobSpawner> MobSpawner;
    [Header("The next scene to go to. \nLeave blank if you just want it to act as a blocker.")]
    [SerializeField] private string nextScene;
    private List<EntityManager> entityManagers;
    private BoxCollider boxCollider;
    private bool allowedToMoveOn;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        entityManagers = new List<EntityManager>();
        if(MobSpawner == null || MobSpawner.Count == 0)
            allowedToMoveOn = false;
        else
            allowedToMoveOn = true;
    }

    public void initializeMobs()
    {
        foreach(MobSpawner mobSpawner in MobSpawner)
        {
            List<EntityManager> list = mobSpawner.SpawnEnemies();
            foreach(EntityManager manager in list)
            {
                entityManagers.Add(manager);
                manager.OnEntityKilledEvent += OnEnemyDeath;
            }
        }
    }

    void OnEnemyDeath()
    {
        entityManagers.RemoveAt(entityManagers.Count-1);
        if(entityManagers.Count == 0)
        {
            allowedToMoveOn = true;
            boxCollider.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(nextScene == null || nextScene.Length == 0)
            return;
        if(other.CompareTag("Player") && allowedToMoveOn)
        {
            SceneManager.LoadScene(nextScene);
        }
    }


}
