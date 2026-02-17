using UnityEngine;

public class AttackState : PlayerState
{
    private int comboStep = 0;
    private float comboTimer;
    private float comboWindow = 0.8f;

    private Collider[] detectedEnemies;
    private float boxCastRange = 4;
    private float boxCastOffset = 1;

    public float damage = 1;

    public AttackState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void Enter()
    {
        controller.AttackPressed = false;
        comboStep = 1;
        PlayAttack(comboStep);

        detectedEnemies = Physics.OverlapBox(controller.transform.position + controller.transform.forward * boxCastOffset, new Vector3(2, 2, boxCastRange / 2), controller.transform.rotation);

        foreach (Collider c in detectedEnemies)
        {
            Damageable dmg = c.gameObject.GetComponentInParent<Damageable>();
            if (dmg == null || c.gameObject.CompareTag("Player")) continue;

            DamageInfo info = new DamageInfo(damage, controller.gameObject);
            dmg.TakeDamage(info);
        }
    }

    public override void HandleInput()
    {
        if (controller.damaged.source != null)
        {
            stateMachine.ChangeState(new HurtState(stateMachine, controller));
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
            stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }
    }

    private void PlayAttack(int step)
    {
        Debug.Log("Attack " + step);

        controller.AnimTrigger("Attack" + step);

        comboTimer = 0;
    }
}