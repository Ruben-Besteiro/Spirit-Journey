using UnityEngine;

public class WallJumpState : PlayerState
{
    public WallJumpState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void Enter()
    {
        if (controller.CheckWall(out RaycastHit hit))
        {
            Vector3 jumpDir = hit.normal*2 + Vector3.up*5;
            controller.velocity = jumpDir.normalized * controller.jumpForce;
        }
    }

    public override void Update()
    {
        controller.ApplyGravity();

        if (controller.velocity.y <= 0)
            stateMachine.ChangeState(new FallState(stateMachine, controller));
    }
}
