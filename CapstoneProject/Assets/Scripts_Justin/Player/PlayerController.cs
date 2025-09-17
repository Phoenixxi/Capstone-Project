using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using lilGuysNamespace;

/// <summary>
/// Takes in input from the player and performs the associated action. Logic is mostly limited to movement and aiming, with combat logic to be handled by other classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Essentials: Do Not Change!")]
    [SerializeField] private GameObject aimDirection;
    [SerializeField] private SwappingManager swappingManager;
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float gravityMultiplier = 1f;

    private Vector3 movementDirection;
    private Vector3 mousePosition;
    private Rigidbody player;
    private bool isGrounded;
    private Camera playerCamera;
    private List<GameObject> charactersListPC;


    public void Start()
    {
        // Will active the first time each character is awake
        charactersListPC = swappingManager.charactersList;
    }

    private void Awake()
    {
        player = GetComponent<Rigidbody>();
        isGrounded = true;
        playerCamera = FindFirstObjectByType<Camera>();
    }

    /// <summary>
    /// Triggers when the four movement keys are pressed
    /// </summary>
    /// <param name="input"></param>
    private void OnMove(InputValue input)
    {
        Vector2 inputtedDirection = input.Get<Vector2>().normalized;
        movementDirection.x = inputtedDirection.x;
        movementDirection.z = inputtedDirection.y;
    }

    /// <summary>
    /// Triggers when the jump button is pressed
    /// </summary>
    private void OnJump()
    {
        if (!isGrounded) return;
        player.linearVelocity = new Vector3(player.linearVelocity.x, jumpVelocity, player.linearVelocity.z);
    }

    private void OnAim(InputValue input)
    {
        Vector2 mouseInput = input.Get<Vector2>();
        mousePosition = playerCamera.ScreenToWorldPoint(new Vector3(mouseInput.x, mouseInput.y, 10f));
        mousePosition.y = aimDirection.transform.position.y;
    }

    private void OnSwapCharacter1()
    {
        // Might need to add Rotation to the swap functions later
        Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        charactersListPC[0].transform.position = currentLocation.position;

        charactersListPC[0].SetActive(true);
        charactersListPC[1].SetActive(false);
        charactersListPC[2].SetActive(false);
        Debug.Log("Character 1 chosen");
    }

    private void OnSwapCharacter2(InputValue input)
    {
        Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        charactersListPC[1].transform.position = currentLocation.position;

        charactersListPC[0].SetActive(false);
        charactersListPC[1].SetActive(true);
        charactersListPC[2].SetActive(false);
        Debug.Log("Character 2 chosen");
    }

    private void OnSwapCharacter3(InputValue input)
    {
        Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        charactersListPC[2].transform.position = currentLocation.position;

        charactersListPC[0].SetActive(false);
        charactersListPC[1].SetActive(false);
        charactersListPC[2].SetActive(true);
        Debug.Log("Character 3 chosen");
    }

    private void FixedUpdate()
    {
        aimDirection.transform.LookAt(mousePosition);
        Vector3 convertedVelocity = new Vector3(movementDirection.x * movementSpeed, player.linearVelocity.y, movementDirection.z * movementSpeed);
        player.linearVelocity = convertedVelocity;
        if (player.linearVelocity.y < 0) player.linearVelocity += Vector3.up * Physics.gravity.y * gravityMultiplier * Time.deltaTime;
    }

    /// <summary>
    /// Sets whether or not the player is currently touching the ground
    /// </summary>
    /// <param name="isGrounded"></param>
    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }
}
