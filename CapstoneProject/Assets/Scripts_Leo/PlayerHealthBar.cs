using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;

public class PlayerHealthBar : MonoBehaviour
{
    public EntityManager entityManager;
    public Image healthBar;

    float maxhp;
    float hp;
    float lerpTime;
    //public Image healthBarBoom, healthBarGloom, healthBarZoom;
    //public float healthZoom, maxHealthZoom = 100;

    /*
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
    */
    private void Start()
    {
        maxhp = entityManager.maxHealth;
    }

    private void Update()
    {
        lerpTime = 2f * Time.deltaTime;
        hp = entityManager.currentHealth;

        updateHPFill();
        updateColor();

        //healthBarZoom.fillAmount = Mathf.Lerp(healthBarZoom.fillAmount, healthZoom / maxHealthZoom, (2f * Time.deltaTime));
        //updateHPBarFill(zoom);
    }

    private void updateHPFill()
    {
        //healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, hp / maxhp, lerpTime);
        healthBar.fillAmount = hp / maxhp;
    }

    private void updateColor()
    {
        healthBar.color = Color.Lerp(Color.red, Color.green, hp / maxhp);
    }
    /*
    private void updateHPBarFill(CharacterStatus x)
    {
        x.hpBar.fillAmount = Mathf.Lerp(x.hpBar.fillAmount, x.hp / x.maxhp, lerpTime);
    }
    */
}
