using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;


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
            if (SteamClient.IsValid)
            {
                var achievement = new Steamworks.Data.Achievement("ACH_DefeatTheHeart");
                if (!achievement.State)
                {
                    achievement.Trigger();
                }
            }
        } else
        {
            if (SteamClient.IsValid)
            {
                var achievement = new Steamworks.Data.Achievement("ACH_BeatLevel1");
                if (!achievement.State)
                {
                    achievement.Trigger();
                }
            }
            SceneManager.LoadScene("Level2-Rework");
        }
    }
}
