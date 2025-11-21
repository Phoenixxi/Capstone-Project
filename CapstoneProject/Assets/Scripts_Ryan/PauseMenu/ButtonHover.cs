using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rectTransform;
    private PauseMenu pauseMenu;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        pauseMenu = GetComponentInParent<PauseMenu>();
    }

    void Start()
    {
        pauseMenu.unPaused += OnUnpause;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.localScale = Vector3.one;
    }

    private void OnUnpause(object sender, EventArgs e)
    {
        rectTransform.localScale = Vector3.one;
    }
}
