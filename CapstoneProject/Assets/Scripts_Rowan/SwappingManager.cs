using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    public string currentCharacter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         foreach (GameObject character in charactersList)
        {
            Debug.Log(character.name);
        }
        currentCharacter = "zoom";
    }

    // Update is called once per frame
    void Update()
    {
        
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

}
