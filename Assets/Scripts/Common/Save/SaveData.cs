using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public List<PlayerModeData> unlockedBenditions;

    public SaveData()
    {
        unlockedBenditions = new List<PlayerModeData>();
    }
}