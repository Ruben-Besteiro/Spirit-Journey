using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;
    [SerializeField] private Damageable damageable;
    private DamageInfo damaged;

    void Awake()
    {
        if (damageable != null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += OnDamageReceived;

        if (damageable == null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += TakeDamage;

        print(damageable.ToString());
    }

    private void OnDestroy()
    {
        if (damageable != null)
           damageable.OnDamaged -= TakeDamage;
    }

    public void OnDamageReceived(DamageInfo info)
    {
        print("Bala: OnDamageReceived");
        damaged = new DamageInfo(info.amount, info.source);
    }

    public void TakeDamage(DamageInfo info)
    {
        if (damaged.source == null || damaged.source.gameObject.CompareTag("Enemy")) return;
        Destroy(gameObject);
    }

    private void Start()
    {
        //Destroy(gameObject, 2);
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
