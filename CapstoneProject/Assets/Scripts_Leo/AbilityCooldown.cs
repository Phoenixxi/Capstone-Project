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

    void Start()
    {
        swapper.SwapCharacterEvent += SwapAbilityIcon;
    }

    private void SwapAbilityIcon(int characterNum)
    {
        switch(characterNum)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
        }
    }

    void Update()
    {
        cooldownImage.fillAmount = ability.GetCooldownRatio();
    }
}
