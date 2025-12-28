using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    public string currentCharacter;
    public Action<int> DeathSwapEvent;
    [SerializeField] public UIPlayerSwap uiPlayerSwap;

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
        if(uiPlayerSwap == null)
        {
            Debug.LogError("UIPlayer Swap not set in Swapping Manager");
            return;
        }
        
        if(charactersList[0] == self)
            uiPlayerSwap.zoomDied();
        if(charactersList[1] == self)
            uiPlayerSwap.boomDied();
        if(charactersList[2] == self)
            uiPlayerSwap.gloomDied();

        
        for(int i = 0; i < charactersList.Count; i++)
        {
            if(charactersList[i] != self && charactersList[i].GetComponent<EntityManager>().isAlive)
            {
                Debug.Log("Swapping to: " + charactersList[i].name);
                if (DeathSwapEvent != null) DeathSwapEvent(i + 1);
                return;
            }
        }
        Debug.Log("All characters are dead.");
        SceneManager.LoadScene("GameOver");
    }

}
