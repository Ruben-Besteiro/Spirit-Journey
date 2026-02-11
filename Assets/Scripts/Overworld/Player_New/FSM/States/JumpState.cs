using UnityEngine;

public class JumpState : PlayerState
{
    public JumpState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

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

        if (controller.AttackPressed)
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
    }
}
