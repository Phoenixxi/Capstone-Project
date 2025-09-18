using UnityEngine;
using lilGuysNamespace;

public class EntityManager : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    [SerializeField] public SwappingManager swappingManager;
    public float currentHealth;
    public bool isAlive = true;
    
    void Start()
    {
        // TEMPORARY- change back to maxHealth later
        currentHealth = 25f;
    }

    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Entity has died.");
            isAlive = false;
            swappingManager.PlayerHasDied(gameObject);
        }
        else
        {
            Debug.Log("Entity took damage. Current health: " + currentHealth);
        }
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Entity healed. Current health: " + currentHealth);
    }
}
