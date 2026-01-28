using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Audio Database")]
public class AudioDatabase : ScriptableObject
{
    [SerializeField] private List<AudioCategory_SO> categories;

    private Dictionary<string, AudioCategory_SO> categoryLookup;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        categoryLookup = new Dictionary<string, AudioCategory_SO>();

        foreach (var category in categories)
        {
            if (category == null) continue;

            string key = category.CategoryName;
            if (!categoryLookup.ContainsKey(key))
                categoryLookup.Add(key, category);
        }
    }

    public AudioCategory_SO GetCategory(string categoryName)
    {
        if (categoryLookup == null || categoryLookup.Count == 0)
            BuildLookup();

        categoryLookup.TryGetValue(categoryName, out var category);
        return category;
    }

    public AudioUnit_SO GetUnit(string categoryName, string unitName)
    {
        var category = GetCategory(categoryName);
        return category?.GetUnit(unitName);
    }

    public AudioUnit_SO GetUnit(string categoryName, int index)
    {
        var category = GetCategory(categoryName);
        return category?.GetUnit(index);
    }
}
