using UnityEngine;

public class JumpState : PlayerState
{
    private PlayerModeManager modeManager;

    public JumpState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) 
    { 
        modeManager = controller.GetComponent<PlayerModeManager>(); 
    }

    public override void Enter()
    {
        controller.Jump();
        controller.AnimTrigger("Jump");
    }

    public override void Update()
    {
        controller.Move();
        controller.ApplyGravity();

        if (controller.GetComponent<CharacterController>().isGrounded)
            stateMachine.ChangeState(new IdleState(stateMachine, controller));

        if (Physics.OverlapSphere(controller.transform.position + controller.transform.forward, controller.attackRange, LayerMask.GetMask("Enemy")).Length > 0)
        {
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
        }

        if (controller.velocity.y <= 0)
            stateMachine.ChangeState(new FallState(stateMachine, controller));
    }
    private void HandleAirAbilities()
    {
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
