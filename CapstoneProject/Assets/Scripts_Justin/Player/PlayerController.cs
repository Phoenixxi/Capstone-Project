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
    [SerializeField] private LayerMask aimLayerMask;
    
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

    //public Transform mouseObject;

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

    public void HealAllCharacters(float healAmount)
    {

       
        foreach(GameObject character in charactersListPC)
        {
            EntityManager entity = character.GetComponentInChildren<EntityManager>();
            entity.Heal(healAmount);
        }
        
        
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

    /// <summary>
    /// Triggers when the user moves their mouse
    /// </summary>
    /// <param name="input"></param>
    private void OnAim(InputValue input)
    {
        Vector2 mouseInput = input.Get<Vector2>();

        Ray ray = playerCamera.ScreenPointToRay(mouseInput);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, aimLayerMask))
        {
            mousePosition = hit.point;
            mousePosition.y = aimDirection.transform.position.y;
        }

        //mouseObject.position = mousePosition;
    }

    /// <summary>
    /// Triggers when the attack button is pressed
    /// </summary>
    private void OnAttack()
    {
        EntityManager currentCharacter = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();
        currentCharacter.Attack(aimDirection.transform.forward, transform.position);
    }

    /// <summary>
    /// Triggers when the character 1 (Zoom) hotkey is pressed
    /// </summary>
    private void OnSwapCharacter1()
    {
        // Check if character is alive
        GameObject zoom = charactersListPC[0];
        EntityManager entity = zoom.GetComponent<EntityManager>();
        if(!entity.isAlive)
        {
            Debug.Log("Zoom is dead and cannot be swapped to.");
            return;
        }

        // Might need to add Rotation to the swap functions later
        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        //zoom.transform.position = currentLocation.position;

        //Activate Zoom
        zoom.SetActive(true);
        // Deactivate the other characters
        charactersListPC[1].SetActive(false);
        charactersListPC[2].SetActive(false);
        Debug.Log("Zoom chosen");
    }

    /// <summary>
    /// Triggers when the character 2 (Boom) hotkey is pressed
    /// </summary>
    /// <param name="input"></param>
    private void OnSwapCharacter2(InputValue input)
    {
        // Check if character is alive
        GameObject boom = charactersListPC[1];
        EntityManager entity = boom.GetComponent<EntityManager>();
        if(!entity.isAlive)
        {
            Debug.Log("Boom is dead and cannot be swapped to.");
            return;
        }

        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        //boom.transform.position = currentLocation.position;

        // Activate Boom
        boom.SetActive(true);
        // Deactivate the other characters
        charactersListPC[0].SetActive(false);
        charactersListPC[2].SetActive(false);
        Debug.Log("Boom chosen");
    }

    /// <summary>
    /// Triggers when the character 3 (Gloom) hotkey is pressed
    /// </summary>
    /// <param name="input"></param>
    private void OnSwapCharacter3(InputValue input)
    {
        GameObject gloom = charactersListPC[2];
        EntityManager entity = gloom.GetComponent<EntityManager>();
        if(!entity.isAlive)
        {
            Debug.Log("Gloom is dead and cannot be swapped to.");
            return;
        }

        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();
        //gloom.transform.position = currentLocation.position;

        // Activate Gloom
        gloom.SetActive(true);
        // Deactivate the other characters
        charactersListPC[0].SetActive(false);
        charactersListPC[1].SetActive(false);
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
