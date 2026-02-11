using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void HandleInput()
    {
        if (controller.MoveInput != Vector2.zero)
            stateMachine.ChangeState(new MoveState(stateMachine, controller));

        if (controller.JumpPressed)
            stateMachine.ChangeState(new JumpState(stateMachine, controller));

        if (controller.AttackPressed)
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
    }

    public override void Update()
    {
        controller.ApplyGravity();
    }
}
