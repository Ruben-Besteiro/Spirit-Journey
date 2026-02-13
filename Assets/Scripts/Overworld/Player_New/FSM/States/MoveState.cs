using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        controller.AnimBool("IsMoving", true);
    }

    public override void HandleInput()
    {
        if (controller.damaged > 0)
        {
            stateMachine.ChangeState(new HurtState(stateMachine, controller, controller.damaged));
            return;
        }

        if (controller.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
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
        controller.Move();
        controller.ApplyGravity();
    }

    public override void Exit() { }
}