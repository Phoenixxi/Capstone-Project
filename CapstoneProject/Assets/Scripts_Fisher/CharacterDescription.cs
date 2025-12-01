using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDescription : MonoBehaviour
{
    [SerializeField] private GameObject ui;

    //void Update()
    //{
    //    Debug.Log("pressing tab");
    //    ui.SetActive(Keyboard.current.tabKey.isPressed);
    //}

    private void OnEnable()
    {
        FindFirstObjectByType<PlayerController>().AbilityScreenPressedEvent += ToggleScreen;
        ui.SetActive(false);
    }

    private void OnDisable()
    {
        FindFirstObjectByType<PlayerController>().AbilityScreenPressedEvent -= ToggleScreen;
    }

    private void ToggleScreen(bool isEnabled)
    {
        ui.SetActive(isEnabled);
    }
}
