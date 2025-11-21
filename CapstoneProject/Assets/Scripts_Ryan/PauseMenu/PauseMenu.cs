using System;
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
    [Header("The UI Elements that need to persist, \nbut disappear when paused")]
    [SerializeField] private List<GameObject> UIElementsPersistent;
    [SerializeField] private List<GameObject> HelpMenu;
    [SerializeField] private PlayerInput input;
    private PlayerInput playerInput;
    private bool isPaused;

    public event EventHandler unPaused;

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
        input.SwitchCurrentActionMap("UI");
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

        foreach(var UIElement in UIElementsPersistent)
        {
            UIElement.SetActive(!active);
        }
    }

    public void Pause()
    {
        if (isPaused)
        {
            SetActive(false);
            BackHelp();
            isPaused = false;
            playerInput.actions.FindActionMap("Player").Enable();
            Time.timeScale = 1f;
            unPaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            SetActive(true);
            isPaused = true;
            playerInput.actions.FindActionMap("Player").Disable();
            Time.timeScale = 0f;
        }
    }

    public void Help()
    {
        foreach(var element in HelpMenu)
        {
            element.SetActive(true);
        }
    }

    public void BackHelp()
    {
        foreach (var element in HelpMenu)
        {
            element.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
