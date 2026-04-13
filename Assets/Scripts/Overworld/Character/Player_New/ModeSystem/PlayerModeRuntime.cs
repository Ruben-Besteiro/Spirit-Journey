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
    {
        if (data == null) return baseValue;
        float stageBonus = (data.upgradeData != null) ? data.upgradeData.speedStage * 0.5f : 0f;
        return baseValue * (data.moveSpeedMultiplier + stageBonus);
    }

    public float ModifyJumpForce(float baseValue)
    {
        if (data == null) return baseValue;
        float stageBonus = (data.upgradeData != null) ? data.upgradeData.speedStage * 0.5f : 0f;
        return baseValue * (data.jumpForceMultiplier + stageBonus);
    }

    public bool CanWallJump() => data.canWallJump;
    public bool CanWallClimb() => data.canWallClimb;
    public bool CanDoubleJump() => data.canDoubleJump;

    public float ModifyDamage(float baseDamage)
    {
        if (data == null) return baseDamage;
        float stageBonus = (data.upgradeData != null) ? data.upgradeData.damageStage * 0.5f : 0f;
        return baseDamage * (data.damageMultiplier + stageBonus);
    }

    public float ModifyRange(float baseRange)
    {
        if (data == null) return baseRange;
        float stageBonus = (data.upgradeData != null) ? data.upgradeData.rangeStage * 0.5f : 0f;
        return baseRange * (data.rangeMultiplier + stageBonus);
    }
}
