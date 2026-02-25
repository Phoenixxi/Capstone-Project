using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossHealthUI : MonoBehaviour
{
    [SerializeField] private Transform bar;
    private Image[] images;
    private float initialScaleX;

    void Awake()
    {
        initialScaleX = bar.localScale.x;
        images = GetComponentsInChildren<Image>();
    }

    void Start()
    {
        FinalBossManagerSingleton.Instance.InitializeFinalBoss += OnInitializeFinalBoss;
        foreach(Image image in images)
        {
            image.enabled = false;
        }
    }

    private void OnInitializeFinalBoss()
    {
        foreach(Image image in images)
        {
            image.enabled = true;
        }
    }

    public void UpdateHealthBar(float remainingHealth, float maxHealth)
    {
        if(remainingHealth < 0f) return;
        float t = remainingHealth/maxHealth;
        Vector3 scale = transform.localScale;
        scale.x = initialScaleX * t;
        bar.localScale = scale;
    }
}
