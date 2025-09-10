using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Takes in input from the player and performs the associated action. Logic is mostly limited to movement and aiming, with combat logic to be handled by other classes
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 1;


    private Vector3 movementDirection;
    private Rigidbody player;

    private void Awake()
    {
        player = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue input)
    {
        Vector2 inputtedDirection = input.Get<Vector2>().normalized;
        movementDirection.x = inputtedDirection.x;
        movementDirection.z = inputtedDirection.y;
    }

    private void FixedUpdate()
    {
        Vector3 convertedVelocity = new Vector3(movementDirection.x * movementSpeed, player.linearVelocity.y, movementDirection.z * movementSpeed);
        player.linearVelocity = convertedVelocity;
    }
}
