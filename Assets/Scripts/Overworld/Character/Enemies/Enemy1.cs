using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1 : OverworldObject
{
    [SerializeField] private Damageable damageable;

    [Header("Movement")]
    protected Vector3 vectorToPlayer;
    protected Coroutine attackCoroutine;

    [Header("Animation")]
    [SerializeField] protected Animator animator;
    [SerializeField] private ParticleSystem hitVFX;

    [Header("Detection")]
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float playerDetectDistance = 100;

    [Header("Health")]
    [SerializeField] protected float maxHP = 2;
    protected float currentHP;

    protected Transform playerTransform;
    protected CharacterController controller;
    protected EnemyStates currentState = EnemyStates.Wait;
    RaycastHit hit;
    protected NavMeshAgent agent;
    DamageInfo damaged;
    [SerializeField] public GameObject bullet;     // Prefab de la bala (solo si utiliza)
    [SerializeField] protected BoxCollider damageBox;

    public bool hasTakenDamageThisFrame = false;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        if (damageable != null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += OnDamageReceived;

        if (damageable == null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += TakeDamage;

        currentHP = maxHP;
    }

    private void OnDestroy()
    {
        if (damageable != null)
            damageable.OnDamaged -= TakeDamage;
    }

    public void OnDamageReceived(DamageInfo info)
    {
        damaged = new DamageInfo(info.amount, info.source);
    }

    protected virtual void Update()
    {
        if (isPaused && agent.enabled)
        {
            if (agent != null && !agent.isStopped) agent.isStopped = true;
            return;
        }

        if (playerTransform != null) vectorToPlayer = playerTransform.position - transform.position;

        switch (currentState)
        {
            case EnemyStates.Wait:
                WaitUpdate();
                break;
            case EnemyStates.Chase:
                ChaseUpdate();
                break;
            case EnemyStates.AttackCooldown:
                AttackUpdate();
                break;
        }
    }

    private void LateUpdate()
    {
        hasTakenDamageThisFrame = false;
    }

    protected virtual void WaitUpdate()
    {
        agent.isStopped = true;

        RaycastHit[] hits = Physics.RaycastAll(transform.position, vectorToPlayer.normalized, playerDetectDistance);

        // Ordenar por distancia por si acaso
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit h in hits)
        {
            // Excluimos ciertas cosas
            if (h.collider.transform.root == transform.root || h.collider.gameObject.CompareTag("Enemy") || h.collider.gameObject.CompareTag("Trigger") || h.collider.gameObject.CompareTag("Bullet")) continue;

            hit = h;

            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Jugador encontrado");
                Debug.DrawRay(transform.position, vectorToPlayer.normalized * playerDetectDistance, Color.green);
                currentState = EnemyStates.Chase;
            }
            else
            {
                Debug.DrawRay(transform.position, vectorToPlayer.normalized * playerDetectDistance, Color.red);
            }
            //print(gameObject.name + " Se encontró " + hit.collider.gameObject.name + " " + hit.collider.gameObject.tag);

            break;
        }

        // Failsafe si por alguna razón el raycast que apunta al jugador no detecta al jugador
        if (hits.Length == 0)
        {
            print(gameObject.name + " No se encontró nada");
            currentState = EnemyStates.Chase;
        }
    }

    protected virtual void ChaseUpdate()
    {
        //print("Estado Chase");

        // Nos persigue usando el Nav Mesh
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);

        if (vectorToPlayer.magnitude <= agent.stoppingDistance + .1f)
        {
            currentState = EnemyStates.AttackCooldown;
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, vectorToPlayer.normalized, playerDetectDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.root == transform.root || h.collider.gameObject.CompareTag("Enemy") || h.collider.gameObject.CompareTag("Trigger") || h.collider.gameObject.CompareTag("Bullet")) continue;

            Debug.DrawRay(transform.position, vectorToPlayer.normalized * h.distance, Color.green);

            if (hits.Length == 0) break;
            if (!h.collider.gameObject.CompareTag("Player") && !h.collider.gameObject.CompareTag("Enemy"))
            {
                currentState = EnemyStates.Wait;
            }

            break;
        }
    }

    protected virtual void AttackUpdate()
    {
        if (attackCoroutine != null) return;
        attackCoroutine = StartCoroutine(IEAttackCooldown());
    }

    private IEnumerator IEAttackCooldown()
    {
        if (agent.enabled) agent.isStopped = true;
        damageBox.enabled = false;
        yield return new WaitForSeconds(1);
        attackCoroutine = null;
        currentState = EnemyStates.Chase;
        damageBox.enabled = true;
        if (agent.enabled) agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponentInParent<PlayerController>().hasTakenDamageThisFrame)
                other.gameObject.GetComponentInParent<Damageable>().TakeDamage(new DamageInfo(1, gameObject));
            StartCoroutine(IEKnockback(-transform.forward));      // Aplicamos knockback al enemigo también para que no haya sandwiches
            currentState = EnemyStates.AttackCooldown;
        }
    }

    protected void TakeDamage(DamageInfo info)
    {
        if (hasTakenDamageThisFrame) return;
        hasTakenDamageThisFrame = true;
        if (damaged.source == null || damaged.source.gameObject.CompareTag("Enemy")) return;

        // Hacer el daño
        currentHP -= damaged.amount;

        SpawnHitVFX(info);
        if (currentHP <= 0)
        {
            GameManager.Instance.kills++;
            Destroy(gameObject);
        }
        else
        {
            Vector3 damageDir = (transform.position - damaged.source.transform.position).normalized;
            StartCoroutine(IEKnockback(damageDir));
        }
    }

    private void SpawnHitVFX(DamageInfo info)
    {
        if (hitVFX == null) return;
        Vector3 dir = (transform.position - info.source.transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        ParticleSystem vfx = Instantiate(hitVFX, transform.position, rot);

        Destroy(vfx.gameObject, vfx.main.duration);
    }

    private IEnumerator IEKnockback(Vector3 dir)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float knockbackSpeed = 15f;

        // Si es el enemigo quien se hace knockback a sí mismo y no el jugador, la velocidad es menor
        if (!hasTakenDamageThisFrame) knockbackSpeed = 10f;

        while (knockbackSpeed > 0)
        {
            agent.Move(dir * Time.deltaTime * knockbackSpeed);
            knockbackSpeed -= Time.deltaTime * 30f;
            yield return null;
        }
        //agent.SetDestination(playerTransform.position);
    }
}