using System;

public static class LocalizationManager
{
    // Event to notify all UI elements when language is changed
    public static event Action OnLanguageChanged;
    private static LocalizationData currentData;

    public static void SetLanguage(LocalizationData newData)
    {
        currentData = newData;
        OnLanguageChanged?.Invoke();
    }

    public static string GetValue(string original)
    {
        if (currentData == null) return original;
        return currentData.GetTranslation(original);
    }
}
