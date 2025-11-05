using System.Collections;
using TMPro;
using UnityEngine;
using ElementType = lilGuysNamespace.EntityData.ElementType;

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
