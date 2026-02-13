using UnityEngine;

public class AttackState : PlayerState
{
    private int comboStep = 0;
    private float comboTimer;
    private float comboWindow = 0.8f;

    public AttackState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void Enter()
    {
        Debug.Log("Enter AttackState");

        // Resetear el input de ataque para evitar bucles
        controller.AttackPressed = false;

        comboStep = 1;
        PlayAttack(comboStep);

        controller.EnableAttackBox();
    }

    public override void HandleInput()
    {
        if (controller.damaged > 0)
        {
            controller.DisableAttackBox(); // Desactivar attackBox antes de cambiar a Hurt
            stateMachine.ChangeState(new HurtState(stateMachine, controller, (int)controller.damaged));
            return;
        }

        if (controller.AttackPressed && comboStep < 3)
        {
            comboStep++;
            PlayAttack(comboStep);
            comboTimer = 0;
        }
    }

    public override void Update()
    {
        comboTimer += Time.deltaTime;

        if (comboTimer >= comboWindow)
        {
            controller.DisableAttackBox();
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }
    }

    private void PlayAttack(int step)
    {
        Debug.Log("Attack " + step);

        controller.AnimTrigger("Attack" + step);

        comboTimer = 0;
    }

    public override void Exit()
    {
        // Asegurarse de que la attackBox queda desactivada al salir
        controller.DisableAttackBox();
    }
}