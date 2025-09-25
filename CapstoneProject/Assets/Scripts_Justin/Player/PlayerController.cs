using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using lilGuysNamespace;
using UnityEngine.SceneManagement;

/// <summary>
/// Takes in input from the player and performs the associated action. Logic is mostly limited to movement and aiming, with combat logic to be handled by other classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Essentials: Do Not Change!")]
    [SerializeField] private GameObject aimDirection;
    [SerializeField] private SwappingManager swappingManager;
    [SerializeField] private LayerMask aimLayerMask;
    
    //[Header("Movement Settings")]
    //[SerializeField] private float movementSpeed = 1f;
    //[SerializeField] private float jumpHeight = 1f;
    //[SerializeField] private float gravity = 10f;

    private Vector2 movementInput;
    private Vector3 mousePosition;
    private CharacterController player;
    private Camera playerCamera;
    private List<GameObject> charactersListPC;
    private EntityManager currentCharacter;

    //public Transform mouseObject;

    public void Start()
    {
        charactersListPC = swappingManager.charactersList;
    }

    private void Awake()
    {
        player = GetComponent<CharacterController>();
        playerCamera = FindFirstObjectByType<Camera>();
        currentCharacter = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();
    }

    public void HealAllCharacters(float healAmount)
    {

       
        foreach(GameObject character in charactersListPC)
        {
            EntityManager entity = character.GetComponentInChildren<EntityManager>();
            entity.Heal(healAmount/3f);
        }
        
    }

     public void TakeDamageWrapper(float damage)
    {
        EntityManager activeChar = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();
        activeChar.TakeDamage(damage);
    }

    public void HealActiveCharacter(float healAmount)
    {
        EntityManager activeChar = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();
        activeChar.Heal(healAmount);
    }

    /// <summary>
    /// Triggers when the four movement keys are pressed
    /// </summary>
    /// <param name="input"></param>
    private void OnMove(InputValue input)
    {
        movementInput = input.Get<Vector2>().normalized;
        currentCharacter.SetInputDirection(movementInput);

    }

    /// <summary>
    /// Triggers when the jump button is pressed
    /// </summary>
    private void OnJump()
    {
        currentCharacter.Jump();
    }

    /// <summary>
    /// Updates the mouse's aiming direction
    /// </summary>
    /// <param name="input"></param>
    private void UpdateMouseAim()
    {
        Vector2 mouseScreenPos = Mouse.current.position.value;

        Ray ray = playerCamera.ScreenPointToRay(mouseScreenPos);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, aimLayerMask))
        {
            mousePosition = hit.point;
            mousePosition.y = aimDirection.transform.position.y;
        }
        aimDirection.transform.LookAt(mousePosition);
        //mouseObject.position = mousePosition;
    }

    /// <summary>
    /// Triggers when the attack button is pressed
    /// </summary>
    private void OnAttack()
    {
        currentCharacter.Attack(aimDirection.transform.forward, transform.position);
    }

    /// <summary>
    /// Triggers when the character 1 (Zoom) hotkey is pressed
    /// </summary>
    private void OnSwapCharacter1()
    {
        if (currentCharacter.AbilityInUse()) return;
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
        entity.SetMovementVelocity(currentCharacter.GetMovementVelocity());
        currentCharacter = entity;
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
        if (currentCharacter.AbilityInUse()) return;
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
        entity.SetMovementVelocity(currentCharacter.GetMovementVelocity());
        currentCharacter = entity;
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
        if (currentCharacter.AbilityInUse()) return;
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
        entity.SetMovementVelocity(currentCharacter.GetMovementVelocity());
        currentCharacter = entity;
        // Deactivate the other characters
        charactersListPC[0].SetActive(false);
        charactersListPC[1].SetActive(false);
        Debug.Log("Character 3 chosen");
    }

    private void OnAbility()
    {
        currentCharacter.UseAbility(movementInput);
    }

    private void Update()
    {
        UpdateMouseAim();
        //FOR TESTING PURPOSES, WILL BE REMOVED LATER
        if (transform.position.y <= -10) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
