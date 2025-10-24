using UnityEngine;
using UnityEngine.UI;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Script that manages the healthbar that shows above the enemy's heads
/// </summary>
public class EnemyHealthbar : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject zoomIcon;
    [SerializeField] private GameObject boomIcon;
    [SerializeField] private GameObject gloomIcon;

    private GameObject? currentIcon;

    private void Start()
    {
        zoomIcon.SetActive(false);
        boomIcon.SetActive(false);
        gloomIcon.SetActive(false);
        GetComponentInParent<EntityManager>().OnHealthUpdatedEvent += OnHealthUpdated;
    }

    /// <summary>
    /// Updates the enemy's healthbar
    /// </summary>
    /// <param name="currentHealth">The enemy's current health</param>
    /// <param name="maxHealth">The enemy's max health</param>
    /// <param name="taggedElement">The element they are now tagged with</param>
    private void OnHealthUpdated(float currentHealth, float maxHealth, ElementType taggedElement)
    {
        healthSlider.value = currentHealth / maxHealth;
        UpdateElementIcon(taggedElement);
    }

    //TODO This function will likely need to be changed when animations for element combinations are added
    /// <summary>
    /// Updates what element icon is being displayed over the health bar
    /// </summary>
    /// <param name="taggedElement">The element the enemy is currently tagged with</param>
    private void UpdateElementIcon(ElementType taggedElement)
    {
        if (currentIcon != null) currentIcon.SetActive(false);
        switch(taggedElement)
        {
            case ElementType.Zoom:
                currentIcon = zoomIcon;
                break;
            case ElementType.Boom:
                currentIcon = boomIcon;
                break;
            case ElementType.Gloom:
                currentIcon = gloomIcon;
                break;
            default:
                currentIcon = null;
                break;
        }
        if (currentIcon != null) currentIcon.SetActive(true);
    }

}
