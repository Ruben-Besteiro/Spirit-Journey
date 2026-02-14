using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<CapsuleCollider>().enabled = false;
            if (!other.gameObject.GetComponent<PlayerController>().hasTakenDamageThisFrame)
                other.gameObject.GetComponent<Damageable>().TakeDamage(new DamageInfo(1, gameObject));
        }
    }
}
