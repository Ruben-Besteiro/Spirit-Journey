public class PlayerModeRuntime
{
    private PlayerModeData data;
    private PlayerController controller;

    public PlayerModeRuntime(PlayerModeData data, PlayerController controller)
    {
        this.data = data;
        this.controller = controller;
    }

    public float ModifyMoveSpeed(float baseValue)
        => baseValue * data.moveSpeedMultiplier;

    public float ModifyJumpForce(float baseValue)
        => baseValue * data.jumpForceMultiplier;

    public bool CanWallJump() => data.canWallJump;
    public bool CanWallClimb() => data.canWallClimb;
    public bool CanDoubleJump() => data.canDoubleJump;

    public float ModifyDamage(float baseDamage)
        => baseDamage * data.damageMultiplier;
}
