using UnityEngine;
using UnityEngine.SceneManagement;
public class HurtState : PlayerState
{
    private float hurtDuration = 1f; // Tiempo que dura el estado de daño
    private float timer;

    public HurtState(PlayerStateMachine stateMachine, PlayerController controller)
        : base(stateMachine, controller)
    { }

    public override void Enter()
    {
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