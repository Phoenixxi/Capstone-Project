using UnityEngine;

public class TurnOn : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void EnableObject()
    {
        gameObject.SetActive(true);
    }
}