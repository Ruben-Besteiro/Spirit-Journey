using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            if (!other.gameObject.GetComponent<PlayerController>().hasTakenDamageThisFrame)
            {
                other.gameObject.GetComponent<Damageable>().TakeDamage(new DamageInfo(damage, gameObject));
            }
        }
    }
}
