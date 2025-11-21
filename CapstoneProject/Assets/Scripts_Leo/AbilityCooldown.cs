using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;
using System;
using System.Collections.Generic;

public class AbilityCooldownDisplay : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private string charName;
    [SerializeField] private Ability ability;

    private Type abilitySubClass;
    //private Dictionary<string, Type> abilities = new Dictionary<string, Type>();

    //public Ability Ability { get => ability; set => ability = value; }


    void Start()
    {
        //ability = entityManager.GetAbility();

        /*
        abilities.add("Boom", typeof(GroundPoundAbility));
        abilities.add("Gloom", typeof(BuffZoneAbility));
        abilities.add("Zoom", typeof(DashAbility));

        ability = abilities[charName];
        */
    }

    void Update()
    {
        //cooldownImage.fillAmount = abilitySubClass.GetCooldownRatio(); 
    }
}
