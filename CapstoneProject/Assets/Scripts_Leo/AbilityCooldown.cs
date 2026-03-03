using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;
using System;
using System.Collections.Generic;
using DG.Tweening.Plugins;

public class AbilityCooldownDisplay : MonoBehaviour
{
    //[SerializeField] private Ability ability;
    //[SerializeField] private SwappingManager swapper;
    [SerializeField] private GameObject zoomIcon;
    [SerializeField] private GameObject boomIcon;
    [SerializeField] private GameObject gloomIcon;
    private Image cooldownImage;

    void Start()
    {
        //swapper.DeathSwapEvent += SwapAbilityIcon;
        SwapAbilityIcon(1);
    }

    private void OnEnable()
    {
        FindFirstObjectByType<PlayerController>().CharacterSwapEvent += SwapAbilityIcon;
        Ability.OnCooldownChangedEvent += UpdateCooldown;
    }

    private void OnDisable()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();
        if (player != null) player.CharacterSwapEvent -= SwapAbilityIcon;
        //FindFirstObjectByType<PlayerController>().CharacterSwapEvent -= SwapAbilityIcon;
        Ability.OnCooldownChangedEvent -= UpdateCooldown;
    }

    private void SwapAbilityIcon(int characterNum)
    {
        switch(characterNum)
        {
            case 1:
                Debug.Log("Zoom swapped to");
                zoomIcon.SetActive(true);
                boomIcon.SetActive(false);
                gloomIcon.SetActive(false);
                cooldownImage = zoomIcon.transform.GetChild(0).GetComponent<Image>();
                break;
            case 2:
                Debug.Log("Boom swapped to");
                zoomIcon.SetActive(false);
                boomIcon.SetActive(true);
                gloomIcon.SetActive(false);
                cooldownImage = boomIcon.transform.GetChild(0).GetComponent<Image>();
                break;
            case 3:
                Debug.Log("Gloom swapped to");
                zoomIcon.SetActive(false);
                boomIcon.SetActive(false);
                gloomIcon.SetActive(true);
                cooldownImage = gloomIcon.transform.GetChild(0).GetComponent<Image>();
                break;
        }
    }

    //void Update()
    //{
    //    cooldownImage.fillAmount = ability.GetCooldownRatio();
    //}

    private void UpdateCooldown(float currentCooldown, float cooldown)
    {
        cooldownImage.fillAmount = currentCooldown / cooldown;
    }
}
