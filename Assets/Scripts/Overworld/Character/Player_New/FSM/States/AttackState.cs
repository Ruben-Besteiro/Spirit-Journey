using UnityEngine;

public class AttackState : PlayerState
{
    private int comboStep = 0;
    private float comboTimer;
    private float comboWindow = 0.48f;

    private Collider[] detectedEnemies;
    private float boxCastRangeZ = 4;
    private float boxCastRangeXY = 1.5f;
    private float boxCastOffset = 1;

    public float damage;

    public AttackState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void Enter()
    {
        // El jugador pega l solo (no hace falta darle al ratón)
        damage = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.ModifyDamage(1);

        var transformation = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.data;
        boxCastRangeXY = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.ModifyRange(transformation.boxCastRangeXY);
        boxCastRangeZ = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.ModifyRange(transformation.boxCastRangeZ);
        boxCastOffset = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.ModifyRange(transformation.boxCastOffset);

        controller.AttackPressed = false;
        comboStep = 1;
        PlayAttack(comboStep);

        detectedEnemies = Physics.OverlapBox(controller.transform.position + controller.transform.forward * boxCastOffset, new Vector3(boxCastRangeXY, boxCastRangeXY, boxCastRangeZ / 2), controller.transform.rotation);

        Color i = Color.red;

        foreach (Collider c in detectedEnemies)
        {
            if (!c.CompareTag("Enemy") && !c.CompareTag("Bullet")) continue;        // Las balas también pueden recibir daño

            i = Color.yellow;

            Damageable dmg = c.gameObject.GetComponentInParent<Damageable>();
            if (dmg == null || c.gameObject.CompareTag("Player")) continue;

            DamageInfo info = new DamageInfo(damage, controller.gameObject);
            dmg.TakeDamage(info);
        }

        DebugBoxDrawer.DrawBox(controller.transform.position + controller.transform.forward * boxCastOffset, new Vector3(boxCastRangeXY, boxCastRangeXY, boxCastRangeZ / 2), controller.transform.rotation, i, 0.5f);
    }

    public override void HandleInput()
    {
        if (controller.damaged.source != null && comboTimer >= comboWindow / 2)
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
        controller.ApplyGravity();
        comboTimer += Time.deltaTime;

        if (comboTimer >= comboWindow / 2)
        {
            if (comboTimer >= comboWindow)
            {
                stateMachine.ChangeState(new IdleState(stateMachine, controller));
            }
            else if (controller.AttackPressed)
            {
                stateMachine.ChangeState(new AttackState(stateMachine, controller));
            }
        }
    }

    private void PlayAttack(int step)
    {
        Debug.Log("Attack " + step);

        //controller.AnimTrigger("Attack" + step);

        comboTimer = 0;
    }

    public override void Exit()
    {

    }
}