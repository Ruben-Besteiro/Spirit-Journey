using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Audio/Audio Category")]
public class AudioCategory_SO : ScriptableObject
{
    [SerializeField] private string categoryName;
    [SerializeField] private List<AudioUnit_SO> audioUnits;

    private Dictionary<string, AudioUnit_SO> lookup;

    public string CategoryName => categoryName;

    private void OnEnable()
    {
        BuildLookup();
    }

    private void BuildLookup()
    {
        lookup = new Dictionary<string, AudioUnit_SO>();

        foreach (var unit in audioUnits)
        {
            if (unit == null) continue;

            string key = unit.name;
            if (!lookup.ContainsKey(key))
                lookup.Add(key, unit);
        }
    }

    /* LOOKUPS */

    public AudioUnit_SO GetUnit(int index)
    {
        if (index < 0 || index >= audioUnits.Count)
            return null;

        return audioUnits[index];
    }

    public AudioUnit_SO GetUnit(string unitName)
    {
        if (lookup == null || lookup.Count == 0)
            BuildLookup();

        lookup.TryGetValue(unitName, out var unit);
        return unit;
    }
}
