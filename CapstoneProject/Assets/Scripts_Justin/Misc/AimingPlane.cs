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
        if(!player.isGrounded && player.transform.position.y >= currentY)
        {
            transform.position = new Vector3(player.transform.position.x, currentY, player.transform.position.z);
        } else
        {
            transform.position = player.transform.position;
            currentY = transform.position.y;
        }
    }
}
