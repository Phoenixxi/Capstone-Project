using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Isekai : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private bool fadeOut = false;
    [SerializeField] private AudioClip FAAHsound;
    private AudioSource audioSource;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if(fadeOut)
        {
            canvasGroup.alpha -= Time.deltaTime;
            if(canvasGroup.alpha <= 0)
            {
                fadeOut = false;
                gameObject.SetActive(false);
                canvasGroup.alpha = 1f;
            }
        }
    }
    void OnEnable()
    {
        fadeOut = true;
        if (audioSource != null && FAAHsound != null)
        {
            audioSource.PlayOneShot(FAAHsound, 1.8f);
        }
    }

}
