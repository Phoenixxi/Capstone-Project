using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public Text healthTextUI;

    private void Start()
    {
        currentHealth = 25;
        healthTextUI.text = "Health: " + currentHealth.ToString();
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        healthTextUI.text =  "Health: " + currentHealth.ToString();
        Debug.Log("Player healed. Current health: " + currentHealth);
    }
}
