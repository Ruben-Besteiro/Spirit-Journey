using UnityEngine;

public class WallClimbState : PlayerState
{
    private PlayerModeManager modeManager;

    public WallClimbState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller)
    {
        modeManager = controller.GetComponent<PlayerModeManager>();
    }

    public override void Enter()
    {
        controller.AnimBool("Hanging", true);
        controller.JumpPressed = false;
    }

    public override void Update()
    {
        controller.ClimbMove();

        if (controller.JumpPressed)
        {
            stateMachine.ChangeState(new WallJumpState(stateMachine, controller));
            return;
        }

        if (controller.GetComponent<CharacterController>().isGrounded)
        {
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }

        if (!modeManager.CanWallClimb())
        {
            stateMachine.ChangeState(new FallState(stateMachine, controller));
        }

        if (!controller.CheckWall(out RaycastHit hit))
        {
            stateMachine.ChangeState(new JumpState(stateMachine, controller));
        }
    }
    public override void Exit()
    { 
        controller.AnimBool("Hanging", false); 
    }
}
