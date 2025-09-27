using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    public string currentCharacter;

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
        foreach(GameObject character in charactersList)
        {
            if(character != self && character.GetComponent<EntityManager>().isAlive)
            {
                Debug.Log("Swapping to: " + character.name);
                Transform currentLocation = self.transform;
                character.transform.position = currentLocation.position;
                character.SetActive(true);
                self.SetActive(false);
                return;
            }
            else // If all characters are dead
            {
                Debug.Log("All characters are dead.");
                return;
            }
        }
    }

}
