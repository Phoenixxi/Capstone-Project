using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("The Player Object in the Scene")]
    [SerializeField] private GameObject Player;
    [Header("The UI Elements")]
    [SerializeField] private List<GameObject> UIElements;
    private PlayerInput playerInput;
    private bool isPaused;

    void Awake()
    {
        isPaused = false;
    }

    void Start()
    {
        if(Player == null)
        {
            Player = GameObject.Find("Player");
        }
        playerInput = Player.GetComponent<PlayerInput>();
        SetActive(false);
    }

    private void OnPause(InputValue input)
    {
        Pause();
    }

    private void SetActive(bool active)
    {
        foreach(var UIElement in UIElements)
        {
            UIElement.SetActive(active);
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            SetActive(false);
            isPaused = false;
            playerInput.actions.FindActionMap("Player").Enable();
            Time.timeScale = 1f;
        }
        else
        {
            SetActive(true);
            isPaused = true;
            playerInput.actions.FindActionMap("Player").Disable();
            Time.timeScale = 0f;
        }
    }

    public void Quit()
    {
        Debug.Log("quit");
#if UNITY_STANDALONE
        {
            Application.Quit();
        }
#elif UNITY_EDITOR
        {
            EditorApplication.isPlaying = false;
        }
#endif
    }
}
