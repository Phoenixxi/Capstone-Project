using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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
    }
}
