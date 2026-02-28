using UnityEngine;

[CreateAssetMenu(menuName = "Player/Modes/New Mode")]
public class PlayerModeData : ScriptableObject
{
    [Header("Stat Modifiers")]
    public float moveSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1f;
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
    public float damageMultiplier = 1f;
    public float boxCastRangeXY;
    public float boxCastRangeZ;
    public float boxCastOffset;
    public float knockback = 15;
}
