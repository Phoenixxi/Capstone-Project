using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLangData", menuName = "Localization/Language Data")]
public class L10nData : ScriptableObject
{
    public string languageName;

    [System.Serializable]
    public struct Entry
    {
        [TextArea] public string originalText;   // English description as Key
        [TextArea] public string translatedText; // Chinese translation as Value
    }

    public List<Entry> entries = new List<Entry>();

    public string GetTranslation(string original)
    {
        var match = entries.Find(e => e.originalText == original);
        return (match.translatedText != null && match.translatedText != "") ? match.translatedText : original;
    }
}