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
    [SerializeField] public CheckpointController checkpointController;

    [Header("Character Swapping VFX")]
    [SerializeField] private GameObject SwitchOffZoom;
    [SerializeField] private GameObject SwitchOffBoom;
    [SerializeField] private GameObject SwitchOffGloom;
    
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
    private int currentCharacterIndex;
    private bool isAttacking;

    //public Transform mouseObject;

    public void Start()
    {
        charactersListPC = swappingManager.charactersList;
        currentCharacterIndex = 0;
        swappingManager.SwapCharacterEvent += OnCharacterSwapForced;
        if(checkpointController == null)
            Debug.LogError("Checkpoint Controller not set in Player Controller on player");
    }

    private void Awake()
    {
        player = GetComponent<CharacterController>();
        playerCamera = FindFirstObjectByType<Camera>();
        currentCharacter = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();
    }

    private void OnEnable()
    {
        swappingManager.SwapCharacterEvent -= OnCharacterSwapForced;
        swappingManager.SwapCharacterEvent += OnCharacterSwapForced;
    }

    private void OnDisable()
    {
        swappingManager.SwapCharacterEvent -= OnCharacterSwapForced;
    }

    public void HealAllCharacters(float healAmount)
    {

       
        foreach(GameObject character in charactersListPC)
        {
            EntityManager entity = character.GetComponentInChildren<EntityManager>();
            entity.Heal(healAmount);
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

        if(Physics.Raycast(ray: ray, hitInfo: out hit, maxDistance: Mathf.Infinity, layerMask: aimLayerMask))
        {
            //Debug.Log(hit.collider.gameObject, hit.collider.gameObject);
            mousePosition = hit.point;
            //mouseObject.position = mousePosition;
            mousePosition.y = aimDirection.transform.position.y;
        }
        aimDirection.transform.LookAt(mousePosition);
        //mouseObject.position = mousePosition;
    }

    /// <summary>
    /// Triggers when the attack button is pressed
    /// </summary>
    private void OnAttack(InputValue input)
    {
        //Debug.Log(input.Get());
        //currentCharacter.Attack(aimDirection.transform.forward, transform.position);
        bool pressed = input.Get<float>() == 1f ? true : false;
        isAttacking = pressed;
    }

    /// <summary>
    /// Triggers when the player swaps to the left or right character. Uses the existing swapping methods so as to not break existing systems
    /// </summary>
    /// <param name="input"></param>
    private void OnSwapCharacter(InputValue input)
    {
        int newCharacterIndex = currentCharacterIndex;
        float pressedValue = input.Get<float>();
        newCharacterIndex += (int)pressedValue;
        if (newCharacterIndex < 0) newCharacterIndex = charactersListPC.Count - 1;
        else if (newCharacterIndex >= charactersListPC.Count) newCharacterIndex = 0;
        Debug.Log($"New character index: {newCharacterIndex}");
        OnCharacterSwapForced(newCharacterIndex + 1);
    }

    /// <summary>
    /// Triggers when the swapping manager forces the player to swap (likely after death)
    /// </summary>
    /// <param name="characterNum">The character number to swap to</param>
    private void OnCharacterSwapForced(int characterNum)
    {
        switch(characterNum)
        {
            case 1:
                OnSwapCharacter1();
                break;
            case 2:
                OnSwapCharacter2();
                break;
            case 3:
                OnSwapCharacter3();
                break;
        }
    }

    /// <summary>
    /// Swaps to the first character (Zoom)
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

        //Figure out last active character and play kicking out VFX
        if(charactersListPC[1].activeSelf == true)  // Boom last active
            Instantiate(SwitchOffBoom, gameObject.transform.position, Quaternion.identity);
        if(charactersListPC[2].activeSelf == true)  // Gloom last active
            Instantiate(SwitchOffGloom, gameObject.transform.position, Quaternion.identity);

        //Activate Zoom
        zoom.SetActive(true);
        entity.SetMovementVelocity(currentCharacter.GetMovementVelocity());
        currentCharacter = entity;
        currentCharacterIndex = 0;
        
        // Deactivate the other characters
        charactersListPC[1].SetActive(false);
        charactersListPC[2].SetActive(false);
    }

    /// <summary>
    /// Swaps to the second character (Boom)
    /// </summary>
    /// <param name="input"></param>
    private void OnSwapCharacter2()
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
        currentCharacterIndex = 1;
        // Deactivate the other characters
        charactersListPC[0].SetActive(false);
        charactersListPC[2].SetActive(false);
    }

    /// <summary>
    /// Swaps to the third character (Gloom)
    /// </summary>
    /// <param name="input"></param>
    private void OnSwapCharacter3()
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
        currentCharacterIndex = 2;
        // Deactivate the other characters
        charactersListPC[0].SetActive(false);
        charactersListPC[1].SetActive(false);
    }

    private void OnAbility()
    {
        currentCharacter.UseAbility(movementInput);
    }

    private void Update()
    {
        UpdateMouseAim();
        if(isAttacking) currentCharacter.Attack(aimDirection.transform.forward, transform.position);
        if (transform.position.y <= -10) {
            if(checkpointController != null)
            {
                Vector3 location = checkpointController.RecentCheckpointLocation();
                transform.position = location;
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
