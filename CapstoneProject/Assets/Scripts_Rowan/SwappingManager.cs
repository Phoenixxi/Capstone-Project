using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    public string currentCharacter;
    public Action<int> SwapCharacterEvent;

    void Start()
    {
         foreach (GameObject character in charactersList)
        {
            Debug.Log(character.name);
        }
        currentCharacter = "zoom";
    }

    public Transform GetCurrentCharacterTransform()
    {
        foreach (GameObject character in charactersList)
        {
            if (character.activeSelf)
            {
                Debug.Log("Current active character: " + character.name);
                return character.transform;
            }
        }
        return null;
    }

    public void PlayerHasDied(GameObject self)
    {
        Debug.Log($"{self} has died", self);
        for(int i = 0; i < charactersList.Count; i++)
        {
            if(charactersList[i] != self && charactersList[i].GetComponent<EntityManager>().isAlive)
            {
                Debug.Log("Swapping to: " + charactersList[i].name);
                //Transform currentLocation = self.transform;
                //character.transform.position = currentLocation.position;
                //character.SetActive(true);
                //self.SetActive(false);
                if (SwapCharacterEvent != null) SwapCharacterEvent(i + 1);
                return;
            }
        }
        Debug.Log("All characters are dead.");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
