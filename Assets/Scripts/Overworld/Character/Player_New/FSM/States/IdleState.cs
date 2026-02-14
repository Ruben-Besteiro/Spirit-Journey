using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller) { }

    public override void Enter()
    { }

    public override void HandleInput()
    {
        if (controller.damaged.source != null)
        {
            stateMachine.ChangeState(new HurtState(stateMachine, controller));
            return;
        }

        if (controller.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeState(new MoveState(stateMachine, controller));
        }

        if (controller.JumpPressed && controller.isGrounded)
        {
            stateMachine.ChangeState(new JumpState(stateMachine, controller));
        }

        if (controller.AttackPressed)
        {
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
        }
    }

    public override void Update()
    {
        controller.ApplyGravity();
    }

    public override void Exit() { }
}