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
        comboStep = 1;
        PlayAttack(comboStep);
    }

    public override void HandleInput()
    {
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
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }
    }

    private void PlayAttack(int step)
    {
        Debug.Log("Attack " + step);
        comboTimer = 0;
    }
}
