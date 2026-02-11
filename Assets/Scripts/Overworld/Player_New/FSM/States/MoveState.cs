using UnityEngine;

public class MoveState : PlayerState
{
    public MoveState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void HandleInput()
    {
        if (controller.MoveInput == Vector2.zero)
            stateMachine.ChangeState(new IdleState(stateMachine, controller));

        if (controller.JumpPressed)
            stateMachine.ChangeState(new JumpState(stateMachine, controller));

        if (controller.AttackPressed)
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
    }

    public override void Update()
    {
        controller.Move();
        controller.ApplyGravity();
    }
}
