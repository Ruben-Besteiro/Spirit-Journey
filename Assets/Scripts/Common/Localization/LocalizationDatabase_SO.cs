using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(menuName = "Localization/Localization Database")]
public class LocalizationDatabase_SO : ScriptableObject
{
    public List<LocalizationEntry> entries = new();

    private Dictionary<string, LocalizationEntry> lookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, LocalizationEntry>();

        foreach (var entry in entries)
        {
            if (!lookup.ContainsKey(entry.key))
                lookup.Add(entry.key, entry);
        }
    }

    public string Get(string key, Language language)
    {
        if (lookup == null || lookup.Count == 0)
            BuildLookup();

        if (lookup.TryGetValue(key, out var entry))
            return entry.Get(language);

        return $"#{key}#";
    }

    /* CSV IMPORT */

    public void ImportFromCSV(string path)
    {
        if (!File.Exists(path))
        {
            Debug.LogError("CSV not found: " + path);
            return;
        }

        entries.Clear();

        string[] lines = File.ReadAllLines(path);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] columns = lines[i].Split(',');

            if (columns.Length < 3)
                continue;

            LocalizationEntry entry = new LocalizationEntry
            {
                key = columns[0],
                english = columns[1],
                spanish = columns[2]
            };

            entries.Add(entry);
        }

        BuildLookup();
        Debug.Log("Localization CSV imported");
    }

    /* CSV EXPORT */

    public void ExportToCSV(string path)
    {
        using StreamWriter writer = new StreamWriter(path);

        writer.WriteLine("key,english,spanish");

        foreach (var entry in entries)
        {
            writer.WriteLine($"{entry.key},{entry.english},{entry.spanish}");
        }

        Debug.Log("Localization CSV exported");
    }
}

[System.Serializable]
public class LocalizationEntry
{
    public string key;
    [TextArea] public string english;
    [TextArea] public string spanish;

    public string Get(Language language)
    {
        return language switch
        {
            Language.Spanish => spanish,
            _ => english
        };
    }
}

public enum Language
{
    English,
    Spanish
}

