using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    public string currentCharacter;
    public Action<int> DeathSwapEvent;
    [SerializeField] public UIPlayerSwap uiPlayerSwap;
    [Header("Death Pause Effect Settings")]
    [SerializeField] private float timePercentage = 0f; //What percentage of the normal speed time should be moving at during the death effect. Set to 0 to pause the game completely
    [SerializeField] private float pauseDuration = 1f;

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

        int characterIndex = -1;
        for(int i = 0; i < charactersList.Count; i++)
        {
            if(charactersList[i] != self && charactersList[i].GetComponent<EntityManager>().isAlive)
            {
                Debug.Log("Swapping to: " + charactersList[i].name);
                //if (DeathSwapEvent != null) DeathSwapEvent(i + 1);
                //return;
                characterIndex = i;
                break;
            }
        }
        if(characterIndex != -1)
        {
            StartCoroutine(DeathPauseCoroutine(characterIndex));
        } else
        {
            Debug.Log("All characters are dead.");
            SceneManager.LoadScene("GameOver");
        }
    }

    private IEnumerator DeathPauseCoroutine(int characterIndex)
    {
        DeathSwapEvent?.Invoke(characterIndex + 1);
        EntityManager entityManager = charactersList[characterIndex].GetComponent<EntityManager>(); //This is ugky, but its the best way to do it without messing with existing systems
        entityManager.SetInviciblity(true);
        float originalTimeScale = Time.timeScale;
        Time.timeScale = timePercentage;
        //TODO Make invicible
        yield return new WaitForSecondsRealtime(pauseDuration);
        Time.timeScale = originalTimeScale;
        entityManager.SetInviciblity(false);
    }


}
