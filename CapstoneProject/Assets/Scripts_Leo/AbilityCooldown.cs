using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;
using System;
using System.Collections.Generic;

public class AbilityCooldownDisplay : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Ability ability;
    [SerializeField] private SwappingManager swapper;
    [SerializeField] private GameObject zoomIcon;
    [SerializeField] private GameObject boomIcon;
    [SerializeField] private GameObject gloomIcon;

    void Start()
    {
        swapper.SwapCharacterEvent += SwapAbilityIcon;
    }

    private void SwapAbilityIcon(int characterNum)
    {
        switch(characterNum)
        {
            case 1:
                Debug.Log("Zoom swapped to");
                break;
            case 2:
                Debug.Log("Boom swapped to");
                break;
            case 3:
                Debug.Log("Gloom swapped to");
                break;
        }
    }

    void Update()
    {
        cooldownImage.fillAmount = ability.GetCooldownRatio();
    }
}
