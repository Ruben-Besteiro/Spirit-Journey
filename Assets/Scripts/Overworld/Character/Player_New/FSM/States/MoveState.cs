using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerStateMachine stateMachine, PlayerController controller)
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

        if (controller.MoveInput == Vector2.zero)
        {
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }

        if (controller.JumpPressed && controller.isGrounded)
        {
            stateMachine.ChangeState(new JumpState(stateMachine, controller));
        }

        if (Physics.OverlapSphere(controller.transform.position + controller.transform.forward, controller.attackRange, LayerMask.GetMask("Enemy")).Length > 0)
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