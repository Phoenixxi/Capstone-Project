using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    private TextMeshProUGUI tmp;
    private string original;

    void Awake() => tmp = GetComponent<TextMeshProUGUI>();

    void OnEnable()
    {
        if (L10nlManager.Instance == null) return;
        if (string.IsNullOrEmpty(original)) original = tmp.text;
        tmp.text = L10nlManager.Instance.GetText(original);
    }
}