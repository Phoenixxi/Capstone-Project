using UnityEngine;
using lilGuysNamespace;

public class EnemyTesting : MonoBehaviour, IEffectable
{
    private float movementSpeed = 2f;
    private Vector3 startPosition;
    public bool shouldMove = false;

    private AbilityData data;
    

    private void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if(shouldMove)
            HandleMove();
    }

    public bool moveRight = true;

    void HandleMove()
    {
        if(moveRight && Vector3.Distance(transform.position, startPosition + (transform.right * 3f)) < 0.01)
            moveRight = false;
        
        if(!moveRight && Vector3.Distance(transform.position, startPosition + (-transform.right * 3f)) < 0.01)
            moveRight = true;

        if(moveRight)
            transform.position += transform.right * movementSpeed * Time.deltaTime;
        else
            transform.position += -transform.right * movementSpeed * Time.deltaTime;
    }


    public void ApplyEffect(AbilityData data)
    {
        this.data = data;
    } 

    public void RemoveEffect()
    {
        data = null;
    }
}
