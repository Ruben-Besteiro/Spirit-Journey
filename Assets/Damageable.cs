using System;
using UnityEngine;
using UnityEngine.Events; // <-- IMPORTANTE: AÑADIR ESTE USING

public class Damageable : MonoBehaviour
{
    private enum DamageMode
    {
        Ignore,
        PlayerDamage,
        EnemyDamage,
        LossGame,
    }

    [Header("Damage Settings")]
    [SerializeField] private DamageMode mode = DamageMode.Ignore;
    [SerializeField] private int damageAmount = 1;
    [SerializeField] private string tagSelf;

    [Header("Events")]
    public UnityEvent<Vector3, int> OnDamageReceived; // Evento que notifica: dirección del golpe, cantidad de daño

    public void ApplyDamage(Vector3 hitDirection, int damage = -1)
    {
        print(mode);
        if (mode == DamageMode.Ignore) return;

        int dmg = damage > 0 ? damage : damageAmount;

        // <--- NUEVO: Disparar el evento ANTES de aplicar la lógica específica
        // Esto permite que otros componentes (enemy manager, player controller) reaccionen al daño

        switch (mode)
        {
            case DamageMode.PlayerDamage:
                TryDamagePlayer(hitDirection, dmg);
                break;

            case DamageMode.EnemyDamage:
                TryDamageEnemy();
                break;

            case DamageMode.LossGame:
                LossGame(hitDirection);
                break;
        }
    }

    // El daño se lo hace al player
    private void TryDamagePlayer(Vector3 dir, int dmg)
    {
        var player = GetComponent<PlayerController>();
        print("Un enemigo está haciendo daño a algo");
        OnDamageReceived?.Invoke(dir, dmg);

        player.Knockback(dir, dmg);
        PlayerStateMachine sm = player.gameObject.GetComponent<PlayerStateMachine>();
        sm.ChangeState(new HurtState(sm, player, dmg));
    }

    // El daño se lo hace al enemigo
    private void TryDamageEnemy()
    {
        if (gameObject.name == "Player") return;

        print("El jugador está haciendo daño a algo ");
        Destroy(gameObject);
    }

    private void LossGame(Vector3 dir)
    {
        // ?
    }
}
