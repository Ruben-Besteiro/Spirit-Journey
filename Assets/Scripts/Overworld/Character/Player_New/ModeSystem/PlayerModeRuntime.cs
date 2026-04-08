public class PlayerModeRuntime
{
    public PlayerModeData data { get; private set; }
    private PlayerController controller;
    public bool IsActive { get; private set; }

    public PlayerModeRuntime(PlayerModeData data, PlayerController controller)
    {
        this.data = data;
        this.controller = controller;
    }

    public void Activate()
    {
        IsActive = true;

        if (data.modelPrefab != null)
        {
            controller.ApplyVisualOverride(data);
        }
    }

    public void Deactivate()
    {
        IsActive = false;
        
        controller.RestoreDefaultVisual();
    }

    public float ModifyMoveSpeed(float baseValue)
        => baseValue * (data.moveSpeedMultiplier + data.upgradeData.speedStage * 0.5f);

    public float ModifyJumpForce(float baseValue)
        => baseValue * (data.jumpForceMultiplier + data.upgradeData.speedStage * 0.5f);

    public bool CanWallJump() => data.canWallJump;
    public bool CanWallClimb() => data.canWallClimb;
    public bool CanDoubleJump() => data.canDoubleJump;

    public float ModifyDamage(float baseDamage)
        => baseDamage * (data.damageMultiplier + data.upgradeData.damageStage * 0.5f);
}
