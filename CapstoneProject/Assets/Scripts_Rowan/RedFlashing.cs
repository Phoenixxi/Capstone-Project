using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedFlashing : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    private bool decrease;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void LowHealthWarning(float speedMultiplier)
    {
        if(decrease)
        {
            canvasGroup.alpha -= Time.deltaTime*speedMultiplier;
            if(canvasGroup.alpha <= 0.3)
                decrease = false;
        }
        else if(!decrease)
        {
            canvasGroup.alpha += Time.deltaTime*speedMultiplier;
            if(canvasGroup.alpha >= 1)
            {
                decrease = true;
            }
        }
    }



}
