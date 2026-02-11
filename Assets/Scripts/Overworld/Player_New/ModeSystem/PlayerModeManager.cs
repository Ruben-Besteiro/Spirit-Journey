using UnityEngine;

public class PlayerModeManager : MonoBehaviour
{
    [SerializeField] private PlayerModeData defaultMode;

    private PlayerModeRuntime currentMode;
    private PlayerController controller;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();

        if (defaultMode != null)
            SetMode(defaultMode);
    }

    public void SetMode(PlayerModeData modeData)
    {
        currentMode = new PlayerModeRuntime(modeData, controller);
    }

    public float ModifyMoveSpeed(float baseValue)
        => currentMode != null ? currentMode.ModifyMoveSpeed(baseValue) : baseValue;

    public float ModifyJumpForce(float baseValue)
        => currentMode != null ? currentMode.ModifyJumpForce(baseValue) : baseValue;

    public bool CanWallJump()
        => currentMode != null && currentMode.CanWallJump();
}

