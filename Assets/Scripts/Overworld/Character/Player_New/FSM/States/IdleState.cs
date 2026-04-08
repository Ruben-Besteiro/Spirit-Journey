using UnityEngine;

public class IdleState : PlayerState
{
    public IdleState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller) { }

    public override void Enter()
    {
        controller.hasDoubleJumped = false;
    }

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
        
        if (Physics.OverlapSphere(controller.transform.position + controller.transform.forward, controller.attackRange, LayerMask.GetMask("Enemy")).Length > 0)
        {
            foreach (Collider enemy in Physics.OverlapSphere(controller.transform.position, controller.attackRange, LayerMask.GetMask("Enemy")))
            {
                Debug.Log(enemy.name);
                /*if (enemy.TryGetComponent<Damageable>(out Damageable damageable))
                {
                    damageable.TakeDamage(new DamageInfo(1, controller.gameObject));
                }*/
            }
            stateMachine.ChangeState(new AttackState(stateMachine, controller));
        }
    }

    public override void Update()
    {
        controller.ApplyGravity();
    }

    public override void Exit() { }
}