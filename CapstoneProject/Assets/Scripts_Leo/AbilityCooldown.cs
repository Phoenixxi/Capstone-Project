using UnityEngine;
using UnityEngine.UI;
using lilGuysNamespace;

public class AbilityCooldownDisplay : MonoBehaviour
{
    [Serialized] public Image cooldownImage;

    float maxCooldown;
    float currentCooldown;

    void Start()
    {
        // set maxCooldown to base cooldown of relevant ability;
        // set currentCooldown to current cooldown of relevant ability;
    }

    void Update()
    {
        // set currentCooldown to current cooldown of relevant ability;
        cooldownImage.fillAmount = currentCooldown / maxCooldown;
    }
}
