using UnityEngine;

public class AttackBox : MonoBehaviour
{
    // He decidido que esto tenga su propio script para evitar comportamientos extraños
    private void OnTriggerEnter(Collider other)
    {
        print("La attack box ha colisionado con " + other.gameObject.name);
        Damageable d = other.gameObject.GetComponent<Damageable>();
        if (d == null || other.gameObject.CompareTag("Player")) return;

        d.ApplyDamage((transform.position - other.transform.position).normalized, 1);
    }
}