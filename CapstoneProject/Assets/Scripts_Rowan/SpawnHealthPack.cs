using UnityEngine;
using lilGuysNamespace;

public class SpawnHealthPack : MonoBehaviour
{
    public GameObject healthPack;
    public float healthPackChance = 25f;

    public void Spawn(Vector3 position)
    {
        float dropGamble = Random.Range(0f, 100f);
        if (dropGamble <= healthPackChance)
            Instantiate(healthPack, position, Quaternion.identity);
    }
}
