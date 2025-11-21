using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;
using System;
using System.Collections.Generic;

public class AbilityCooldownDisplay : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private Ability ability;

    void Update()
    {
        cooldownImage.fillAmount = ability.GetCooldownRatio();
    }
}
