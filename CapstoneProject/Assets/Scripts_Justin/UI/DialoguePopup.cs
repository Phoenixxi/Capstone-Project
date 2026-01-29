using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Script for dialogue/tutorial popups that appear throughout the level
/// </summary>
public class DialoguePopup : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
   // [SerializeField] private CanvasGroup transparency;

    [Header("Dialogue Popup Settings")]
    [SerializeField] private float fadeTime;
    [SerializeField] private string dialogue;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        //text.alpha = 0f;
       // transparency.alpha = 0f;
        //text.text = dialogue;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeText(1));
    }

    private void OnTriggerExit(Collider other)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeText(0f));
    }

    /// <summary>
    /// Adjusts the text's alpha over the course of fadeTime
    /// </summary>
    /// <param name="alpha">The alpha the text should end on</param>
    /// <returns></returns>
    private IEnumerator FadeText(float alpha)
    {
        //float startingAlpha = text.alpha;
       // float startingAlpha = transparency.alpha;
        float currentTime = 0f;
        while(currentTime <= fadeTime)
        {
            currentTime += Time.deltaTime;
            //text.alpha = Mathf.Lerp(startingAlpha, alpha, currentTime / fadeTime);
            //transparency.alpha = Mathf.Lerp(startingAlpha, alpha, currentTime / fadeTime);
            yield return null;
        }
    }
}
