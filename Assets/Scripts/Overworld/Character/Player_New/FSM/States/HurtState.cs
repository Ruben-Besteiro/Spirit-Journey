using UnityEngine;
using UnityEngine.SceneManagement;
public class HurtState : PlayerState
{
    private float hurtDuration = .5f; // Tiempo que dura el estado de daño
    private float timer;

    public HurtState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller)
    { }

    public override void Enter()
    {
        // Hacer el daño
        controller.currentHP -= controller.damaged.amount;

        //Knockback
        if (controller.damaged.source != null)
        {
            Vector3 damageDir = (controller.transform.position - controller.damaged.source.transform.position).normalized;
            controller.Knockback(damageDir);
        }

        // Restablecer el daño pendiente a 0
        controller.damaged = new DamageInfo(0, null);

        timer = 0;
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        controller.ApplyGravity();

        // Esperar a que termine el tiempo de hurt para decidir si matar al jugador o devolverle el control
        if (timer >= hurtDuration)
        {
            if (controller.currentHP <= 0 && timer >= hurtDuration) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            else stateMachine.ChangeState(new IdleState(stateMachine, controller));
        }
    }
}