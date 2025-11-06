using System.Collections;
using TMPro;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

/// <summary>
/// Script for floating damage numbers that appear when entities are attacked
/// </summary>
public class DamageNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    [Header("Damage Text Settings")]
    [SerializeField] private Color normalColor;
    [SerializeField] private Color zoomColor;
    [SerializeField] private Color boomColor;
    [SerializeField] private Color gloomColor;
    [SerializeField] private float floatDistance;
    [SerializeField] private Vector3 spawnOffset;

    private void Awake()
    {
        int otherDamageNumbers = FindObjectsByType<DamageNumber>(FindObjectsSortMode.None).Length - 1;
        transform.position += spawnOffset + 0.5f * otherDamageNumbers * spawnOffset;
       
    }

    /// <summary>
    /// Displays the damage and element inflicted upon an entity
    /// </summary>
    /// <param name="damage">The amount of damage to display</param>
    /// <param name="element">The damage's element</param>
    /// <param name="time">How long the text should appear for</param>
    public void ShowDamage(int damage, ElementType element, float time)
    {
        text.text = damage.ToString();
        switch(element)
        {
            case ElementType.Zoom:
                text.color = zoomColor;
                break;
            case ElementType.Boom:
                text.color = boomColor;
                break;
            case ElementType.Gloom:
                text.color = gloomColor;
                break;
            default:
                text.color = normalColor;
                break;
        }
        StartCoroutine(ShowDamageCoroutine(time));
    }

    /// <summary>
    /// Displays the damage text and then fades it out over time
    /// </summary>
    /// <param name="time">How long it takes for the damage text to disappear</param>
    /// <returns></returns>
    private IEnumerator ShowDamageCoroutine(float time)
    {
        float currentTime = 0f;
        float startingY = text.transform.position.y;
        float endingY = startingY + floatDistance;
        Vector3 currentPosition = new Vector3(text.transform.position.x, startingY, text.transform.position.z);
        while(currentTime < time)
        {
            currentTime += Time.deltaTime;
            text.alpha = Mathf.Lerp(1, 0f, currentTime / time);
            currentPosition.y = Mathf.Lerp(startingY, endingY, currentTime / time);
            text.transform.position = currentPosition;
            yield return null;
        }
        Destroy(gameObject);
    } 
}
