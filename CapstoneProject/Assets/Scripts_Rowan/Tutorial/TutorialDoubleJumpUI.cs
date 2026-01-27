using UnityEngine;

public class TutorialDoubleJumpUI : MonoBehaviour
{
    [SerializeField] private GameObject doubleJumpCanvas;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            doubleJumpCanvas.SetActive(true);
            Destroy(gameObject);
        }
    }
}
