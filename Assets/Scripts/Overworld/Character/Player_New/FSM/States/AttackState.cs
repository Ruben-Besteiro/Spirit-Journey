using UnityEngine;

public class AttackState : PlayerState
{
    private int comboStep = 0;
    private float comboTimer;
    private float comboWindow = 0.8f;

    private Collider[] detectedEnemies;
    private float boxCastRangeZ = 4;
    private float boxCastRangeXY = 1.5f;
    private float boxCastOffset = 1;
    GameObject hitboxVisual;        // Esta hitbox es solo visual. No tiene collider
    Renderer hitboxRenderer;

    public float damage = 1;

    public AttackState(PlayerStateMachine sm, PlayerController controller)
        : base(sm, controller) { }

    public override void Enter()
    {
        hitboxVisual = GameObject.Find("Visual Hitbox");
        hitboxRenderer = hitboxVisual.GetComponent<Renderer>();
        hitboxRenderer.enabled = false;         // No funciona
        var transformation = controller.GetComponent<PlayerModeManager>().currentRuntimeMode.data;
        boxCastRangeXY = transformation.boxCastRangeXY;
        boxCastRangeZ = transformation.boxCastRangeZ;
        boxCastOffset = transformation.boxCastOffset;

        controller.AttackPressed = false;
        comboStep = 1;
        PlayAttack(comboStep);

        detectedEnemies = Physics.OverlapBox(controller.transform.position + controller.transform.forward * boxCastOffset, new Vector3(boxCastRangeXY, boxCastRangeXY, boxCastRangeZ / 2), controller.transform.rotation);

        foreach (Collider c in detectedEnemies)
        {
            if (!c.CompareTag("Enemy")) continue;

            // Activamos la hitbox visualmente para dar feedback si le damos a algo
            hitboxVisual.transform.position = controller.transform.position + controller.transform.forward * boxCastOffset;
            hitboxVisual.transform.localScale = new Vector3(boxCastRangeXY, boxCastRangeXY, boxCastRangeZ);
            hitboxVisual.transform.rotation = controller.transform.rotation;
            Color color = hitboxRenderer.material.color;
            color.a = 0.1f;
            hitboxRenderer.material.color = color;
            hitboxRenderer.enabled = true;

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

    public override void Exit()
    {
        hitboxRenderer.enabled = false;
    }
}