using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDescription : MonoBehaviour
{
    [SerializeField] private GameObject ui;

    void Update()
    {
        Debug.Log("pressing tab");
        ui.SetActive(Keyboard.current.tabKey.isPressed);
    }
}
