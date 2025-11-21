using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndOfLevel : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemies;
    [SerializeField] private string nextScene;
    private bool allowedToMoveOn;

    void Awake()
    {
        if(nextScene == null)
        {
            Debug.LogError("Next Scene is not set");
        }

        if(enemies == null || enemies.Count == 0)
            allowedToMoveOn = false;
        else
            allowedToMoveOn = true;
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

    void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && allowedToMoveOn)
        {
            SceneManager.LoadScene(nextScene);
        }
    }


}
