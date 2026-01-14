using UnityEngine;

public class AimingPlane : MonoBehaviour
{
    private CharacterController player;
    private float currentY;
    void Awake()
    {
        player = GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(player.isGrounded)
        {
            currentY = player.transform.position.y;
        } else if(player.transform.position.y >= currentY)
        {
            transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
        }
    }
}
