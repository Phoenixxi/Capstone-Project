using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBlocker : MonoBehaviour
{
    [Header("List of enemies that need to be defeated \nbefore moving on.")]
    [SerializeField] private List<GameObject> enemies;
    [Header("The next scene to go to. \nLeave blank if you just want it to act as a blocker.")]
    [SerializeField] private string nextScene;
    private BoxCollider boxCollider;
    private bool allowedToMoveOn;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        if(enemies == null || enemies.Count == 0)
            allowedToMoveOn = false;
        else
            allowedToMoveOn = true;
    }

    void Start()
    {
        foreach(GameObject enemy in enemies)
        {
            EntityManager entityManager = enemy.GetComponent<EntityManager>();
            entityManager.OnEntityKilledEvent += OnEnemyDeath;
        }
    }

    void Update()
    {
        List<int> indexes = new List<int>();
        for(int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i] == null)
                indexes.Add(i);
        }

        foreach(int i in indexes)
        {
            enemies.RemoveAt(indexes[i]);
        }

        allowedToMoveOn = enemies.Count > 0 ? false : true;
    }

    void OnEnemyDeath()
    {
        enemies.RemoveAt(0);
        if(enemies.Count == 0)
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
