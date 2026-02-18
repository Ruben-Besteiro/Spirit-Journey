using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float damage;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<CapsuleCollider>().enabled = false;
            if (!other.gameObject.GetComponent<PlayerController>().hasTakenDamageThisFrame)
            {
                other.gameObject.GetComponent<Damageable>().TakeDamage(new DamageInfo(damage, gameObject));
                Destroy(gameObject);
            }
        }
    }
}
