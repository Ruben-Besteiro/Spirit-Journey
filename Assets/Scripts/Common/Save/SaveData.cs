using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public bool[] unlockedBenditions;

    public SaveData()
    {
        unlockedBenditions = new bool[] { false, false };
    }
}