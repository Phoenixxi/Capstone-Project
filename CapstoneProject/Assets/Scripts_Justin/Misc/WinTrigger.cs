using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    [SerializeField] private CanvasGroup fade;
    [SerializeField] private float fadeDuration = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float currentFadeTime = 0f;
        while(currentFadeTime < fadeDuration)
        {
            currentFadeTime += Time.deltaTime;
            fade.alpha = Mathf.Lerp(0f, 1f, currentFadeTime / fadeDuration);
            yield return null;
        }
        if (SceneManager.GetActiveScene().name == "Level2-Rework")
        {
            SceneManager.LoadScene("WinScreen");
        } else
        {
            SceneManager.LoadScene("Level2-Rework");
        }
    }
}
