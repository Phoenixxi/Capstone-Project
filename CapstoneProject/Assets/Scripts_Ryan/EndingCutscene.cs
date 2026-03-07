using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Steamworks;

public class EndingCutscene : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.loopPointReached += onVideoEnd;
    }

    private void onVideoEnd(VideoPlayer vp)
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
    }
}
