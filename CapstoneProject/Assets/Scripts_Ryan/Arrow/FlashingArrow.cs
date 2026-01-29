using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class FlashingArrow : MonoBehaviour
{
    [SerializeField] private float flashDuration = 2.0f;
    [SerializeField] private float flashInterval = 0.3f;
    private UnityEngine.UI.Image image;

    void Awake()
    {
        image = GetComponent<UnityEngine.UI.Image>();
        image.enabled = false;
    }
    public void StartFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        image.enabled = true;
        float timer = 0f;

        while(timer < flashDuration)
        {
            image.enabled = !image.enabled;
            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        image.enabled = false;
    }
}
