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
                
            case 2:

            case 3:

        }
    }

    void Update()
    {
        cooldownImage.fillAmount = ability.GetCooldownRatio();
    }
}
