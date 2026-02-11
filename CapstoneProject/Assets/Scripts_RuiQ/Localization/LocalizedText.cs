using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    private TextMeshProUGUI textComp;
    private string original;

    void Awake()
    {
        textComp = GetComponent<TextMeshProUGUI>();
        original = textComp.text; // Record the English text in Inspector
    }

    void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= Refresh;
    }

    void Refresh()
    {
        // Try to replace text with translation from dictionary
        textComp.text = LocalizationManager.GetValue(original);
    }
}