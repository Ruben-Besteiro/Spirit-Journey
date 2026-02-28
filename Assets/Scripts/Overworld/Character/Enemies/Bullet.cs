using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    Damageable damageable;

    private void Start()
    {
        Destroy(gameObject, 2);

        if (damageable != null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += ctx => Destroy(gameObject);
    }

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
