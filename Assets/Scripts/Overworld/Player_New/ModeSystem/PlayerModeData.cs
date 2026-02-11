using UnityEngine;

[CreateAssetMenu(menuName = "Player/Modes/New Mode")]
public class PlayerModeData : ScriptableObject
{
    [Header("Stat Modifiers")]
    public float moveSpeedMultiplier = 1f;
    public float jumpForceMultiplier = 1f;

    [Header("Abilities")]
    public bool canWallJump;
    public bool canWallClimb;
    public bool canDoubleJump;

    [Header("Combat")]
    public float damageMultiplier = 1f;
}
