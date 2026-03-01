using UnityEngine;

public class NPCJUMPTEST : MonoBehaviour
{
    [SerializeField] private float height = 1f;
    [SerializeField] private float speed = 1f;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float t = Mathf.PingPong(Time.time * speed, 1f);
        float eased = Mathf.Sin(t * Mathf.PI * 0.5f);
        transform.position = startPos + Vector3.up * eased * height;
    }
}