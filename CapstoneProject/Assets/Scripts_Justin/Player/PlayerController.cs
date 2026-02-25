using Unity.VisualScripting;

using UnityEngine;

using UnityEngine.InputSystem;

using System.Collections.Generic;

using lilGuysNamespace;

using UnityEngine.SceneManagement;

using System;



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

    [SerializeField] private UIPlayerSwap uiPlayerSwap;



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

    private bool hoverButtonPressed;

    private int keyCount = 2;
    private int secondKeyCount = 0;



    //public Transform mouseObject;

    public Action<int> CharacterSwapEvent;

    public Action<bool> AbilityScreenPressedEvent;



    public Action Pause;



    public void Start()

    {

        charactersListPC = swappingManager.charactersList;

        currentCharacterIndex = 0;

        swappingManager.DeathSwapEvent += OnCharacterSwapForced;

        foreach (GameObject character in charactersListPC) character.GetComponent<EntityManager>().SetHealthToFull();

        if (checkpointController == null)

            Debug.LogError("Checkpoint Controller not set in Player Controller on player");


    // Skip tutorial if player reattempts level after dying
    if(SceneManager.GetActiveScene().name == "Level01_LouieScene"){
        if(StaticSceneData.playerReattempting)
            transform.position = new Vector3(105.289001f,-8.64999962f,-26.7910004f);
        else
        transform.position = new Vector3(-68.7699966f,-2.477f,-44.2700005f);
    }

    }



    private void Awake()

    {

        player = GetComponent<CharacterController>();

        playerCamera = FindFirstObjectByType<Camera>();

        currentCharacter = swappingManager.GetCurrentCharacterTransform().GetComponent<EntityManager>();

    }



    private void OnEnable()

    {

        swappingManager.DeathSwapEvent -= OnCharacterSwapForced;

        swappingManager.DeathSwapEvent += OnCharacterSwapForced;

    }



    private void OnDisable()

    {

        swappingManager.DeathSwapEvent -= OnCharacterSwapForced;

    }



    public void HealAllCharacters(float healAmount)

    {

        uiPlayerSwap.AllPlayersHealed();



        foreach (GameObject character in charactersListPC)

        {

            EntityManager entity = character.GetComponentInChildren<EntityManager>();

            entity.Heal(healAmount);

        }

    }

    public void HealAllLivingCharacters(float healAmount)
    {
        foreach (GameObject character in charactersListPC)
        {
            EntityManager entity = character.GetComponentInChildren<EntityManager>();
            if(entity.isAlive)
            {
                Debug.Log("Healed " + entity.entityName + " from reaction for " + 1f);
                entity.Heal(healAmount);
            }
        }
    }

    public void HealLivingCharactersFromReaction()
    {
        foreach (GameObject character in charactersListPC)
        {
            EntityManager entity = character.GetComponentInChildren<EntityManager>();
            if(!entity.isAlive)
                return;
            else
            {
                if(entity.standingInGloomBuffZone){ // BUG HERE FIX LATER
                    HealAllLivingCharacters(1f);
                }
                else
                {
                    float missingHealth = entity.maxHealth - entity.currentHealth;
                    float healthPercent = entity.currentHealth / entity.maxHealth; // 1.0 at full, 0.0 at empty

                    // As health is lower, healing increases.
                    // To decrease healing, decrease the maximum coefficient (the first one) and increase the minimum coefficient (the second one). 
                    // To increase healing, do the opposite.
                    float coefficient = Mathf.Lerp(0.11f, 0.1f, healthPercent);

                    Debug.Log("Healed " + entity.entityName + " for " + (missingHealth * coefficient) + " who had current health of " + entity.currentHealth + " and missing health of " + missingHealth);
                    entity.Heal(missingHealth * coefficient);
                }
            }
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

        if (!canMove) return; //for dialogue

        movementInput = input.Get<Vector2>().normalized;

        currentCharacter.SetInputDirection(movementInput);



    }



    /// <summary>

    /// Triggers when the jump button is pressed

    /// </summary>

    private void OnJump()

    {

        if (!canMove) return; //for dialogue

        currentCharacter.Jump();

    }

    /// <summary>
    /// Triggers when the player interacts with the hover button
    /// </summary>
    /// <param name="input"></param>
    private void OnHover(InputValue input)
    {
        if (input.isPressed) Debug.Log("Start Hovering");
        else Debug.Log("Stop Hovering");
        hoverButtonPressed = input.isPressed;
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



        if (Physics.Raycast(ray: ray, hitInfo: out hit, maxDistance: Mathf.Infinity, layerMask: aimLayerMask))

        {

            //Debug.Log(hit.collider.gameObject, hit.collider.gameObject);

            mousePosition = hit.point;

            //mouseObject.position = mousePosition;

            //mousePosition.y = aimDirection.transform.position.y;

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

        if (!canMove) return; //for dialogue

        bool pressed = input.Get<float>() == 1f ? true : false;

        isAttacking = pressed;

    }

    public void DecrementKeyCount()
    {
        keyCount = 1;
        secondKeyCount++;
    }

    /// <summary>

    /// Triggers when the player swaps to the left or right character. Uses the existing swapping methods so as to not break existing systems

    /// </summary>

    /// <param name="input"></param>

    private void OnSwapCharacter(InputValue input)

    {

        float pressedValue = input.Get<float>();

        Scene currentScene = SceneManager.GetActiveScene();
        if(currentScene.name == "Level01_LouieScene")
        {
            if(transform.position.x > 88)
                secondKeyCount = 3;
        } else
        {
            secondKeyCount = 3;
        }

        // Cannot swap characters at this time
        if ((secondKeyCount != 3 && keyCount == 2) || (pressedValue == -1 && secondKeyCount != 3))
        {
            return;
        }


        int newCharacterIndex = currentCharacterIndex;

        if (uiPlayerSwap != null)

        {
            if (currentCharacter.AbilityInUse()) return;
            {
                if (pressedValue == -1 && secondKeyCount == 3) // Q

                    uiPlayerSwap.swapImageLocation(-1);

                if (pressedValue == 1) // E

                    uiPlayerSwap.swapImageLocation(1);
                    keyCount = 2;
            }


        }

        else { Debug.LogError("uiPlayerSwap not set in player controller"); }

        do

        {

            newCharacterIndex += (int)pressedValue;

            if (newCharacterIndex < 0) newCharacterIndex = charactersListPC.Count - 1;

            else if (newCharacterIndex >= charactersListPC.Count) newCharacterIndex = 0;



        }

        while (!charactersListPC[newCharacterIndex].GetComponent<EntityManager>().isAlive);

        //Debug.Log($"New character index: {newCharacterIndex}");

        if (newCharacterIndex != currentCharacterIndex) OnCharacterSwapForced(newCharacterIndex + 1);

    }



    /// <summary>

    /// Triggers when the swapping manager forces the player to swap (likely after death)

    /// </summary>

    /// <param name="characterNum">The character number to swap to</param>

    private void OnCharacterSwapForced(int characterNum)

    {

        switch (characterNum)

        {

            case 1:

                uiPlayerSwap.stateOne();

                OnSwapCharacter1();

                break;

            case 2:

                uiPlayerSwap.stateTwo();

                OnSwapCharacter2();

                break;

            case 3:

                uiPlayerSwap.stateThree();

                OnSwapCharacter3();

                break;

        }

        CharacterSwapEvent?.Invoke(characterNum);

    }



    /// <summary>

    /// Swaps to the first character (Zoom)

    /// </summary>

    private void OnSwapCharacter1()

    {

        if (currentCharacter.AbilityInUse() && currentCharacter.isAlive) return;

        // Check if character is alive

        GameObject zoom = charactersListPC[0];

        EntityManager entity = zoom.GetComponent<EntityManager>();

        if (!entity.isAlive)

        {

            Debug.Log("Zoom is dead and cannot be swapped to.");

            return;

        }



        // Might need to add Rotation to the swap functions later

        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();

        //zoom.transform.position = currentLocation.position;



        //Figure out last active character and play kicking out VFX

        if (charactersListPC[1].activeSelf == true)  // Boom last active

            Instantiate(SwitchOffBoom, gameObject.transform.position, Quaternion.identity);

        if (charactersListPC[2].activeSelf == true)  // Gloom last active

            Instantiate(SwitchOffGloom, gameObject.transform.position, Quaternion.identity);



        //Activate Zoom

        entity.spriteRenderer.color = Color.white;
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

        if (!entity.isAlive)

        {

            Debug.Log("Boom is dead and cannot be swapped to.");

            return;

        }



        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();

        //boom.transform.position = currentLocation.position;



        //Figure out last active character and play kicking out VFX

        if (charactersListPC[0].activeSelf == true)  // Zoom last active

            Instantiate(SwitchOffZoom, gameObject.transform.position, Quaternion.identity);

        if (charactersListPC[2].activeSelf == true)  // Gloom last active

            Instantiate(SwitchOffGloom, gameObject.transform.position, Quaternion.identity);



        // Activate Boom

        entity.spriteRenderer.color = Color.white;
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

        if (!entity.isAlive)

        {

            Debug.Log("Gloom is dead and cannot be swapped to.");

            return;

        }



        //Transform currentLocation = swappingManager.GetCurrentCharacterTransform();

        //gloom.transform.position = currentLocation.position;



        //Figure out last active character and play kicking out VFX

        if (charactersListPC[0].activeSelf == true)  // Zoom last active

            Instantiate(SwitchOffZoom, gameObject.transform.position, Quaternion.identity);

        if (charactersListPC[1].activeSelf == true)  // Boom last active

            Instantiate(SwitchOffBoom, gameObject.transform.position, Quaternion.identity);



        // Activate Gloom
        entity.spriteRenderer.color = Color.white;
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



    private bool canMove = true;



    public void SetCanMove(bool value)

    {

        canMove = value;

        movementInput = Vector2.zero;

        currentCharacter.SetInputDirection(Vector2.zero);

    }





    private void Update()

    {

        UpdateMouseAim();

        if (isAttacking) currentCharacter.Attack(aimDirection.transform.forward, transform.position);
        currentCharacter.ToggleHover(hoverButtonPressed);
        if (transform.position.y <= -22)
        {

            //if(checkpointController != null)

            //{

            //    Vector3 location = checkpointController.RecentCheckpointLocation();

            //    transform.position = location;

            //} else

            //{

            //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            //}

            SendToCheckpoint();

        }

    }



    /// <summary>

    /// Sends the player back to the previous checkpoint if a checkpoint controller exists, otherwise reloads the scene

    /// </summary>

    public void SendToCheckpoint()

    {

        if (checkpointController != null)

        {

            Vector3 location = checkpointController.RecentCheckpointLocation();

            transform.position = location;

        }

        else

        {

            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }

    }



    private void OnAbilityScreen(InputValue input)

    {

        float pressedValue = input.Get<float>();

        AbilityScreenPressedEvent?.Invoke(pressedValue == 1f);

    }



    private void OnPause(InputValue input)

    {

        Pause?.Invoke();

    }

}