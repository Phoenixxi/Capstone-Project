using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterDescription : MonoBehaviour
{
    [SerializeField] private GameObject ui;
    private PlayerController pc;

    //void Update()
    //{
    //    Debug.Log("pressing tab");
    //    ui.SetActive(Keyboard.current.tabKey.isPressed);
    //}

    void Awake()
    {
        pc = FindFirstObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        pc.AbilityScreenPressedEvent += ToggleScreen;
        ui.SetActive(false);
    }

    private void OnDisable()
    {
        pc.AbilityScreenPressedEvent -= ToggleScreen;
    }

    private void ToggleScreen(bool isEnabled)
    {
        ui.SetActive(isEnabled);
    }
}
