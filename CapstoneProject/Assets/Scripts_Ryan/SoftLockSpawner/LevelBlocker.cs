using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ArrowDirection
{
    Right, Up, Down, None
}

public class LevelBlocker : MonoBehaviour
{
    [SerializeField] private ArrowDirection arrowDirection;
    [Header("List of mob spawners beyond this level blocker and before the next level blocker")]
    [SerializeField] private List<MobSpawner> MobSpawner;
    [Header("The next scene to go to. \nLeave blank if you just want it to act as a blocker.")]
    [SerializeField] private string nextScene;
    private List<EntityManager> entityManagers;
    private BoxCollider boxCollider;
    private bool allowedToMoveOn;
    private bool triggered;
    private FlashingArrow arrow;
    private EnemyCounter enemyCounter;
    public event Action<EntityManager> EnemySpawned;
    private int count = 0;

    void Awake()
    {
        triggered = false;
        boxCollider = GetComponent<BoxCollider>();
        entityManagers = new List<EntityManager>();
        if(MobSpawner == null || MobSpawner.Count == 0)
            allowedToMoveOn = false;
        else
            allowedToMoveOn = true;
        
        EnemySpawned += OnEnemySpawned;
    }

    void Start()
    {
        switch(arrowDirection)
        {
            case ArrowDirection.Right:
                arrow = GameObject.Find("RightArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.Up:
                arrow = GameObject.Find("UpArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.Down:
                arrow = GameObject.Find("DownArrow").GetComponent<FlashingArrow>();
                break;
            case ArrowDirection.None:
                arrow = null;
                break;
        }

        enemyCounter = GameObject.Find("EnemyCounter").GetComponent<EnemyCounter>();
    }

    public void initializeMobs()
    {
        if(!triggered)
        {
            foreach(MobSpawner mobSpawner in MobSpawner)
            {
                if(mobSpawner == null)
                {
                    continue;
                }

                count += mobSpawner.GetMobCount();
            }

            foreach(MobSpawner mobSpawner in MobSpawner)
            {
                if(mobSpawner == null)
                {
                    continue;
                }

                mobSpawner.StartCoroutine(mobSpawner.SpawnEnemiesCoroutine(EnemySpawned));
            }
            enemyCounter.initializeCount(count);
            triggered = true;
        }
    }

    void OnEnemySpawned(EntityManager entityManager)
    {
        entityManagers.Add(entityManager);
        entityManager.OnEntityKilledEvent += OnEnemyDeath;
    }

    void OnEnemyDeath()
    {
        count--;
        enemyCounter.decreaseCount();
        if(count <= 0)
        {
            allowedToMoveOn = true;
            boxCollider.isTrigger = true;
            if(arrow != null)
            {
                arrow.StartFlash();
            }
            enemyCounter.disableText();
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
