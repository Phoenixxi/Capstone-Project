using UnityEngine;
using UnityEngine.SceneManagement;

public class TestTeleporter : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player") return;
        if(SceneManager.GetActiveScene().name == "RuiScene(GYM)")
        {
            SceneManager.LoadScene("SkylerLevelDesignTempScene");
        } else
        {
            SceneManager.LoadScene("RuiScene(GYM)");
        }
    }
}
