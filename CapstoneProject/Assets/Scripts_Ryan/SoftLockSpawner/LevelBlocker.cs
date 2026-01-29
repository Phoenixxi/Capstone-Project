using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    private FlashingArrow arrow;

    private int count = 0;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        entityManagers = new List<EntityManager>();
        if(MobSpawner == null || MobSpawner.Count == 0)
            allowedToMoveOn = false;
        else
            allowedToMoveOn = true;
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
        count++;
        UnityEngine.Debug.Log($"Count: {count}");
        entityManagers.RemoveAt(entityManagers.Count-1);
        if(entityManagers.Count == 0)
        {
            allowedToMoveOn = true;
            boxCollider.isTrigger = true;
            if(arrow != null)
            {
                arrow.StartFlash();
            }
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
