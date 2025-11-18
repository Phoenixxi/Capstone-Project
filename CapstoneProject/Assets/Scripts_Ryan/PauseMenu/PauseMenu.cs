using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("Background Game Object in Children")]
    [SerializeField] private GameObject Background;
    [Header("Continue Button in Children")]
    [SerializeField] private GameObject ContinueBtn;
    [Header("The Player Object in the Scene")]
    [SerializeField] private GameObject Player;
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
        Background.SetActive(active);
        ContinueBtn.SetActive(active);
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
}
