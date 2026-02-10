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

    // Update is called once per frame
    void Update()
    {

        if(decrease)
        {
            canvasGroup.alpha -= Time.deltaTime;
            if(canvasGroup.alpha <= 0.2)
                decrease = false;
        }
        else if(!decrease)
        {
            canvasGroup.alpha += Time.deltaTime;
            if(canvasGroup.alpha >= 1)
            {
                decrease = true;
            }
        }
        
    }
}
