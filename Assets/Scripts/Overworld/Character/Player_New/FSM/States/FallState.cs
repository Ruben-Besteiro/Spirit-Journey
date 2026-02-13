using UnityEngine;

public class FallState : PlayerState
{
    private PlayerModeManager modeManager;

    public FallState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller)
    {
        modeManager = controller.GetComponent<PlayerModeManager>();
    }

    public override void Enter()
    {
        controller.ResetFallSpeed();
    }

    public override void Update()
    {
        controller.Move();
        controller.ApplyGravity();

        if (controller.GetComponent<CharacterController>().isGrounded)
            stateMachine.ChangeState(new IdleState(stateMachine, controller));

        if (controller.AttackPressed)
            stateMachine.ChangeState(new AttackState(stateMachine, controller));

        HandleAirAbilities();
    }

    public override void HandleInput()
    {
        if (controller.damaged.source != null)
        {
            stateMachine.ChangeState(new HurtState(stateMachine, controller));
            return;
        }
    }

    private void HandleAirAbilities()
    {
        if (controller.JumpPressed)
        {
            if (modeManager.CanDoubleJump() && !controller.hasDoubleJumped)
            {
                controller.hasDoubleJumped = true;
                stateMachine.ChangeState(new JumpState(stateMachine, controller));
            }
        }

        if (controller.CheckWall(out RaycastHit hit))
        {
            if (modeManager.CanDoubleJump() && !controller.hasDoubleJumped)
            {
                controller.hasDoubleJumped = true;
                stateMachine.ChangeState(new JumpState(stateMachine, controller));
            }
            if (controller.JumpPressed)
            {
                if (modeManager.CanWallClimb())
                {
                    stateMachine.ChangeState(new WallClimbState(stateMachine, controller));
                }

                if (modeManager.CanWallJump())
                {
                    stateMachine.ChangeState(new WallJumpState(stateMachine, controller));
                }
            }
            
        }
    }
}
