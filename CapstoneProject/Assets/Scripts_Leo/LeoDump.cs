using System;
using UnityEngine.UI;
/**
 * This file acts as a scrapyard for unused code snippets in case I want them later for some reason.
 * You never know.
**/
public class LeoDump
{
    /** Health Bar Scraps **/
    /*
    public Image healthBarBoom, healthBarGloom, healthBarZoom;
    public float healthZoom, maxHealthZoom = 100;

    CharacterStatus boom, gloom, zoom;

    private class CharacterStatus
    {
        public Image hpBar;
        public float hp, maxhp = 100;

        public CharacterStatus(Image bar)
        {
            hpBar = bar;
        }
    }

    private void Update()
    {
        //I tried a try catch block to see if we could avoid the other characters' health starting out appearing empty by 
        //having it not update when currentHealth is void, but it didn't do anything.
        try
        {
            lerpTime = 4f * Time.deltaTime;
            currentHP = entityManager.currentHealth;

            UpdateColor();

            healthBarZoom.fillAmount = Mathf.Lerp(healthBarZoom.fillAmount, healthZoom / maxHealthZoom, (2f * Time.deltaTime));
            updateHPBarFill(zoom);
        }
        catch { }
    }

    private void updateHPBarFill(CharacterStatus x)
    {
        x.hpBar.fillAmount = Mathf.Lerp(x.hpBar.fillAmount, x.hp / x.maxhp, lerpTime);
    }
    */

    /** Ability UI Scraps **/
    /*
    [SerializeField] private Image cooldownImage;
    //[SerializeField] private EntityManager entityManager;
    [SerializeField] private string charName;
    //[SerializeField] private Ability ability;

    private Type abilitySubClass;
    private Dictionary<string, Type> abilities = new Dictionary<string, Type>();

    //public Ability Ability { get => ability; set => ability = value; }


    void Start()
    {
        //ability = entityManager.GetAbility();

        
        abilities.add("Boom", typeof(GroundPoundAbility));
        abilities.add("Gloom", typeof(BuffZoneAbility));
        abilities.add("Zoom", typeof(DashAbility));

        abilitySubClass = abilities[charName];
    }

    void Update()
    {
        cooldownImage.fillAmount = abilitySubClass.GetCooldownRatio(); 

    }
    */
}
