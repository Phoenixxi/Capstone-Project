// All comments in English as per your preference.
using UnityEngine;
using UnityEditor;
using TMPro; // Need this for TextMeshPro
using UnityEngine.UI;
using System.Reflection;

public class DeepFinder : EditorWindow
{
    private string targetText = "SPACE BAR - DOUBLE JUMP";

    [MenuItem("L10n/Deep Finder (暴力溯源)")]
    public static void ShowWindow() => GetWindow<DeepFinder>("Deep Finder");

    void OnGUI()
    {
        targetText = EditorGUILayout.TextField("Find this exact text:", targetText);

        if (GUILayout.Button("Start Deep Search (全方位搜寻)"))
        {
            Search();
        }
    }

    private void Search()
    {
        Debug.Log($"<color=yellow>--- Searching for: {targetText} ---</color>");
        bool found = false;

        // 1. Search in Scene UI Components (TMP & Legacy Text)
        var tmpComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();
        foreach (var t in tmpComponents)
        {
            if (t.text.Contains(targetText))
            {
                Debug.Log($"<color=cyan>[Found in TMP Component]</color> Object: {t.gameObject.name} | Path: {GetPath(t.transform)}", t.gameObject);
                found = true;
            }
        }

        // 2. Search in all Script Fields (MonoBehaviours)
        var scripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
        foreach (var s in scripts)
        {
            if (s == null) continue;
            FieldInfo[] fields = s.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var f in fields)
            {
                if (f.FieldType == typeof(string))
                {
                    string val = (string)f.GetValue(s);
                    if (val != null && val.Contains(targetText))
                    {
                        Debug.Log($"<color=green>[Found in Script Variable]</color> Script: {s.GetType().Name} on {s.gameObject.name} (Field: {f.Name})", s.gameObject);
                        found = true;
                    }
                }
            }
        }

        if (!found) Debug.Log("<color=red>Not found anywhere! Try searching for a shorter keyword.</color>");
    }

    private string GetPath(Transform current)
    {
        if (current.parent == null) return current.name;
        return GetPath(current.parent) + "/" + current.name;
    }
}