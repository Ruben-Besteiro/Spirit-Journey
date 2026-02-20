using UnityEngine;
using UnityEngine.SceneManagement;
public class HurtState : PlayerState
{
    private float hurtDuration = 1f; // Tiempo que dura el estado de da�o
    private float timer;

    public HurtState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller)
    { }

    public override void Enter()
    {
        // Restablecer el da�o pendiente a 0
        controller.damaged = new DamageInfo(0, null);

        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        controller.ApplyGravity();

        if (timer >= hurtDuration)
        {
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }
    }
}