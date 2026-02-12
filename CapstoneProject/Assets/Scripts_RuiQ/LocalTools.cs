//using UnityEngine;
//using UnityEditor;
//using TMPro;
//using System.IO;
//using System.Text;
//using System.Collections.Generic;

//public class L10nTools : EditorWindow
//{
//    [MenuItem("TD_Tools/1. Export Scene to CSV")]
//    public static void Export()
//    {
//        string path = EditorUtility.SaveFilePanel("Save CSV", "", "SceneText.csv", "csv");
//        if (string.IsNullOrEmpty(path)) return;

//        StringBuilder sb = new StringBuilder();
//        sb.AppendLine("Original,Translation"); // CSV Header

//        HashSet<string> found = new HashSet<string>();
//        var texts = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

//        foreach (var t in texts)
//        {
//            string val = t.text.Trim().Replace("\n", " ").Replace(",", "，");
//            if (!string.IsNullOrEmpty(val) && !found.Contains(val))
//            {
//                sb.AppendLine($"{val},");
//                found.Add(val);
//            }
//        }
//        File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
//        Debug.Log("Success! Extracted " + found.Count + " lines.");
//    }

//    [MenuItem("TD_Tools/2. Import CSV to Asset")]
//    public static void Import()
//    {
//        string path = EditorUtility.OpenFilePanel("Select Translated CSV", "", "csv");
//        if (string.IsNullOrEmpty(path)) return;

//        LocalizationData data = ScriptableObject.CreateInstance<LocalizationData>();
//        string[] lines = File.ReadAllLines(path);

//        for (int i = 1; i < lines.Length; i++)
//        {
//            string[] split = lines[i].Split(',');
//            if (split.Length < 2) continue;
//            data.entries.Add(new LocalizationData.Entry { originalText = split[0].Trim(), translatedText = split[1].Trim() });
//        }

//        AssetDatabase.CreateAsset(data, "Assets/CN_Data.asset");
//        AssetDatabase.SaveAssets();
//        Debug.Log("Created CN_Data asset in Assets folder.");
//    }
//}
