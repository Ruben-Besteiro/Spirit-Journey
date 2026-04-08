using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterUpgradeData", menuName = "Game Data/Character Upgrade Data")]
public class CharacterUpgradeData : ScriptableObject
{
    // Las mejoras van por niveles del 0 al 3
    [SerializeField] public int rangeStage;
    [SerializeField] public int damageStage;
    [SerializeField] public int speedStage;
}