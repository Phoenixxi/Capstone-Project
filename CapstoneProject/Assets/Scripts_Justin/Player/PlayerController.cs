using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Takes in input from the player and performs the associated action. Logic is mostly limited to movement and aiming, with combat logic to be handled by other classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1f;
    [SerializeField] private float jumpVelocity = 1f;
    [SerializeField] private float gravityMultiplier = 1f;

    private Vector3 movementDirection;
    private Rigidbody player;
    private bool isGrounded;


    private void Awake()
    {
        player = GetComponent<Rigidbody>();
        isGrounded = true;
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

    private void FixedUpdate()
    {
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
