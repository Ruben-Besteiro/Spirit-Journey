using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponentInParent<PlayerController>().hasTakenDamageThisFrame)
            {
                other.gameObject.GetComponentInParent<Damageable>().TakeDamage(new DamageInfo(damage, gameObject));
            }
            Destroy(gameObject);
        }
    }
}
