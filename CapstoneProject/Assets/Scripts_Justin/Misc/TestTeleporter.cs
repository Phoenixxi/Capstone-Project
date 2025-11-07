using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTeleporter : MonoBehaviour
{
    [SerializeField] private string scene1Name;
    [SerializeField] private string scene2Name;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        if(SceneManager.GetActiveScene().name == scene1Name)
        {
            SceneManager.LoadScene(scene2Name);
        } else
        {
            SceneManager.LoadScene(scene1Name);
        }
    }
}
