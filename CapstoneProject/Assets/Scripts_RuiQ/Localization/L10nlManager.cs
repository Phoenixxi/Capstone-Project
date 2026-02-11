using UnityEngine;
using System.Collections.Generic;

public class L10nManager : MonoBehaviour
{
    public static L10nManager Instance;
    private Dictionary<string, string> dictionary = new Dictionary<string, string>();
    public TextAsset csvFile; // 拖入你翻译好的 CSV 文件

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }
        LoadCSV();
    }

    void LoadCSV()
    {
        if (csvFile == null) return;
        string[] lines = csvFile.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] row = lines[i].Split(',');
            if (row.Length >= 2)
            {
                string key = row[0].Trim().Replace("\\n", "\n");
                string value = row[1].Trim().Replace("\\n", "\n");
                if (!dictionary.ContainsKey(key)) dictionary.Add(key, value);
            }
        }
    }

    public string GetText(string key) => dictionary.TryGetValue(key.Trim(), out string val) ? val : key;
}