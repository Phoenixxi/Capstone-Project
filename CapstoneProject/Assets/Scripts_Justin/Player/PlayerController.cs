using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Takes in input from the player and performs the associated action. Logic is mostly limited to movement and aiming, with combat logic to be handled by other classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Essentials: Do Not Change!")]
    [SerializeField] private GameObject aimDirection;
    
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float gravityMultiplier = 1f;

    private Vector3 movementDirection;
    private Vector3 mousePosition;
    private Rigidbody player;
    private bool isGrounded;
    private Camera playerCamera;

    //EVERYTHING INVOLVING WEAPONS WILL BE REMOVED ONCE PROPER ENTITY SCRIPT IMPLEMENTATION IS ADDED
    public float attackCooldown;
    public int damage;
    public GameObject projectile;
    private Weapon weapon;


    private void Awake()
    {
        player = GetComponent<Rigidbody>();
        isGrounded = true;
        playerCamera = FindFirstObjectByType<Camera>();
        //TODO: Delete the line below this
        weapon = new RangedWeapon(attackCooldown, damage, projectile);
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

    private void OnAttack()
    {
        Debug.Log("Attack button pressed");
        //TODO Delete lines below this
        if (weapon is RangedWeapon) (weapon as RangedWeapon).UpdateWeaponTransform(aimDirection.transform.forward, transform.position);
        weapon.Attack();
    }

    private void OnSwapCharacter1()
    {
        Debug.Log("Character 1 chosen");
    }

    private void OnSwapCharacter2(InputValue input)
    {
        Debug.Log("Character 2 chosen");
    }

    private void OnSwapCharacter3(InputValue input)
    {
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
