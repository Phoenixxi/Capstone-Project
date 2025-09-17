using UnityEngine;
using lilGuysNamespace;
using System.Collections.Generic;

public class SwappingManager : MonoBehaviour
{
    [SerializeField] public List<GameObject> charactersList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         foreach (GameObject character in charactersList)
        {
            Debug.Log(character.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
