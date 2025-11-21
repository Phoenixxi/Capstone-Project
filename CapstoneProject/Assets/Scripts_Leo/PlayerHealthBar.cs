using System.Collections;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ElementType = lilGuysNamespace.EntityData.ElementType;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private EntityManager entityManager;
    [SerializeField] private Image healthBar;

    private float maxHP;
    private float currentHP;
    private Coroutine healthFillCoroutine;
    

    void Start()
    {
        //maxHP = entityManager.maxHealth;
        //currentHP = maxHP;
        entityManager.OnHealthUpdatedEvent += UpdateHealth;
    }

    //I replaced this with the event-based functionality seen below, but I kept the method commented in case you wanted to compare
    //private void Update()
    //{
    //    //I tried a try catch block to see if we could avoid the other characters' health starting out appearing empty by 
    //    //having it not update when currentHealth is void, but it didn't do anything.
    //    try
    //    {
    //        lerpTime = 4f * Time.deltaTime;
    //        currentHP = entityManager.currentHealth;

    //        UpdateHPFill();
    //        UpdateColor();

    //        healthBarZoom.fillAmount = Mathf.Lerp(healthBarZoom.fillAmount, healthZoom / maxHealthZoom, (2f * Time.deltaTime));
    //        updateHPBarFill(zoom);
    //    }
    //    catch { }
    //}

    private void UpdateHealth(float currentHealth, float maxHealth, ElementType element)
    {
        maxHP = maxHealth;
        currentHP = currentHealth;

        UpdateHPFill();
        UpdateColor();
    }

    private void UpdateHPFill()
    {
        //healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, currentHP / maxHP, lerpTime);
        if (healthFillCoroutine != null) StopCoroutine(healthFillCoroutine);
        healthFillCoroutine = StartCoroutine(UpdateHPFillCoroutine(currentHP, maxHP));

    }

    /// <summary>
    /// Gradually updates the health bar fill to match the player's current amount of health. This will run until the fill amount is correct, or the coroutine is stopped
    /// </summary>
    /// <param name="currentHealth">The player's new amount of health</param>
    /// <param name="maxHealth">The maximum amount of health the player has</param>
    /// <returns></returns>
    private IEnumerator UpdateHPFillCoroutine(float currentHealth, float maxHealth)
    {
        float currentLerpTime = 0f;
        float healthRatio = currentHealth / maxHealth;
        float startingFill = healthBar.fillAmount;
        while(healthBar.fillAmount != healthRatio)
        {
            currentLerpTime += Time.deltaTime * 4f;
            healthBar.fillAmount = Mathf.Lerp(startingFill, healthRatio, currentLerpTime);
            yield return null;
        }
    }

    private void UpdateColor()
    {
        healthBar.color = Color.Lerp(Color.red, Color.green, currentHP / maxHP);
    }

    private void OnDestroy()
    {
        if (entityManager != null) entityManager.OnHealthUpdatedEvent -= UpdateHealth;
    }
}
