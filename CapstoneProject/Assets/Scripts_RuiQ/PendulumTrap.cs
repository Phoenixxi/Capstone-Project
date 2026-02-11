using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(Rigidbody))]
public class PendulumTrap : MonoBehaviour
{
    [Header("⚙️ 摆动设置")]
    public Vector3 swingAxis = new Vector3(0, 0, 1);
    public float speed = 1.5f;
    public float angleLimit = 75f;
    public float timeOffset = 0f;

    [Header("💥 击飞设置 (重点)")]
    [Tooltip("推力大小，建议设大一点，比如 20 或 30")]
    public float pushForce = 25f;

    [Tooltip("击飞持续时间(秒)，时间越长飞得越远")]
    public float pushDuration = 0.25f;

    private Rigidbody rb;
    private Quaternion startRotation;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
        startRotation = transform.rotation;
    }

    void FixedUpdate()
    {
        float currentAngle = Mathf.Sin((Time.time + timeOffset) * speed) * angleLimit;
        Quaternion rotationOffset = Quaternion.Euler(swingAxis * currentAngle);
        rb.MoveRotation(startRotation * rotationOffset);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            CharacterController playerCC = collision.gameObject.GetComponent<CharacterController>();

            if (playerCC != null)
            {
                Vector3 dir = (collision.transform.position - transform.position).normalized;

                dir += Vector3.up * 0.2f;
                dir.Normalize();

                StartCoroutine(PushPlayerRoutine(playerCC, dir));
            }
        }
    }

    IEnumerator PushPlayerRoutine(CharacterController cc, Vector3 direction)
    {
        float timer = 0f;

        while (timer < pushDuration)
        {

            if (cc == null) yield break;

            float currentForce = Mathf.Lerp(pushForce, 0, timer / pushDuration);

            cc.Move(direction * currentForce * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null; 
        }
    }
}