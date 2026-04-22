using UnityEngine;
using lilGuysNamespace;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class GraveAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject BoomFolder;
    [SerializeField] private GameObject ZoomFolder;
    [SerializeField] private GameObject GloomFolder;
    [SerializeField] private GameObject AllCharacterFolder;

    public void PlayBoomAnimation()
    {
        BoomFolder.SetActive(true);
        BoomFolder.GetComponentInChildren<Animator>().SetTrigger("Play");
        StartCoroutine(DisableFolders());
    }

    public void PlayZoomAnimation()
    {
        ZoomFolder.SetActive(true);
        ZoomFolder.GetComponentInChildren<Animator>().SetTrigger("Play");
        StartCoroutine(DisableFolders());
    }

    public void PlayGloomAnimation()
    {
        GloomFolder.SetActive(true);
        GloomFolder.GetComponentInChildren<Animator>().SetTrigger("Play");
        StartCoroutine(DisableFolders());
    }

    public void PlayAllCharacterAnimations()
    {
        FindFirstObjectByType<PlayerController>().gameObject.SetActive(false);
        AllCharacterFolder.SetActive(true);
        foreach (Animator animator in AllCharacterFolder.GetComponentsInChildren<Animator>())
        {
            animator.SetTrigger("Play");
        }
        StartCoroutine(GoToEndScene());
    }

    IEnumerator GoToEndScene()
    {
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator DisableFolders()
    {
        yield return new WaitForSeconds(1f);
        BoomFolder.SetActive(false);
        ZoomFolder.SetActive(false);
        GloomFolder.SetActive(false);
    }
}
