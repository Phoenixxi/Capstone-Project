using UnityEngine;
using lilGuysNamespace;

public class EntityManager : MonoBehaviour
{
    [SerializeField] public float maxHealth = 100f;
    public float currentHealth;
    
    void Start()
    {
        // TEMPORARY- change back to maxHealth later
        currentHealth = 25f;
    }

    void Update()
    {
        
    }

    public void Heal(float heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Entity healed. Current health: " + currentHealth);
    }
}
