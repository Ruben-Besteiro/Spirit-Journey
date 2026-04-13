using UnityEngine;

[CreateAssetMenu(menuName = "Player/Modes/New Mode")]
public class PlayerModeData : ScriptableObject
{
    [Header("Upgrade Data")]
    [SerializeField] public CharacterUpgradeData upgradeData;

    [Header("Stat Modifiers")]
    public float moveSpeedMultiplier = 1f; // + upgradeData.speedStage * 0.5f;
    public float jumpForceMultiplier = 1f; // + upgradeData.speedStage * 0.5f;
    public float rangeMultiplier = 1f; // + upgradeData.rangeStage * 0.5f;
    internal float staminaDrainPerSecond = 10f;

    [Header("Visual")]
    public Sprite inactiveSprite;
    public Sprite activeSprite;
    public GameObject modelPrefab;

    [Header("Abilities")]
    public bool canWallJump;
    public bool canWallClimb;
    public bool canDoubleJump;

    [Header("Combat")]
    public float damageMultiplier = 1f; // + upgradeData.damageStage * 0.5f;
    public float boxCastRangeXY = 1f; // + upgradeData.rangeStage * 0.5f;
    public float boxCastRangeZ = 1f; // + upgradeData.rangeStage * 0.5f;
    public float boxCastOffset = 1f; // + upgradeData.rangeStage * 0.5f;
    public float knockback = 15f; // + upgradeData.damageStage * 5f;
}
