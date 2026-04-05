using UnityEngine;
using lilGuysNamespace;
using System.Collections;
using System;

public class GraveAnimationController : MonoBehaviour
{
    [SerializeField] private GameObject BoomFolder;
    [SerializeField] private GameObject ZoomFolder;
    [SerializeField] private GameObject GloomFolder;

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

    IEnumerator DisableFolders()
    {
        yield return new WaitForSeconds(2f);
        BoomFolder.SetActive(false);
        ZoomFolder.SetActive(false);
        GloomFolder.SetActive(false);
    }
}
