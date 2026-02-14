using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : OverworldObject
{
    [SerializeField] private Damageable damageable;

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    Vector3 vectorToPlayer;
    protected Coroutine attackCoroutine;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Detection")]
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float playerDetectDistance = 20;

    [Header("Health")]
    [SerializeField] protected float maxHP = 2;
    protected float currentHP;

    protected Transform playerTransform;
    protected CharacterController controller;
    protected EnemyStates currentState = EnemyStates.Wait;
    protected Vector3 velocity;
    RaycastHit hit;
    protected NavMeshAgent agent;
    DamageInfo damaged;
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
        Debug.Log($"Enemy damaged: {info.amount}");
    }

    void Update()
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
                AttackCooldownUpdate();
                break;
        }
    }

    private void LateUpdate()
    {
        hasTakenDamageThisFrame = false;
    }

    protected virtual void WaitUpdate()
    {
        Debug.DrawRay(transform.position, vectorToPlayer * playerDetectDistance, Color.red);
        if (Physics.Raycast(transform.position, vectorToPlayer, out hit, playerDetectDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Jugador encontrado");
                currentState = EnemyStates.Chase;
            }
        }
    }


    protected virtual void ChaseUpdate()
    {
        // Nos persigue usando el Nav Mesh
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);

        if (vectorToPlayer.magnitude <= agent.stoppingDistance + .1f)
        {
            currentState = EnemyStates.AttackCooldown;
        }

        Debug.DrawRay(transform.position, vectorToPlayer * hit.distance, Color.red);
        if (Physics.Raycast(transform.position, vectorToPlayer, out hit, playerDetectDistance))
        {
            if (!hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("Enemy"))
            {
                agent.isStopped = true;
                currentState = EnemyStates.Wait;
            }
        }
    }

    protected virtual void AttackCooldownUpdate()
    {
        if (attackCoroutine != null) return;
        attackCoroutine = StartCoroutine(IEAttackCooldown());
    }

    protected virtual IEnumerator IEAttackCooldown()
    {
        if (agent.enabled) agent.isStopped = true;
        damageBox.enabled = false;
        yield return new WaitForSeconds(2);
        attackCoroutine = null;
        currentState = EnemyStates.Chase;
        damageBox.enabled = true;
        if (agent.enabled) agent.isStopped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<PlayerController>().hasTakenDamageThisFrame)
            {
                other.gameObject.GetComponent<Damageable>().TakeDamage(new DamageInfo(1, gameObject));
            }
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
        //Debug.Log("Enemy damaged by " + info.source.name + " -> " + currentHP + " remaining");

        if (currentHP <= 0) Destroy(gameObject);
        else
        {
            Vector3 damageDir = (transform.position - damaged.source.transform.position).normalized;
            StartCoroutine(IEKnockback(damageDir));
        }
    }

    private IEnumerator IEKnockback(Vector3 dir)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float knockbackSpeed = 15f;

        while (knockbackSpeed > 0)
        {
            agent.Move(dir * Time.deltaTime * knockbackSpeed);
            knockbackSpeed -= Time.deltaTime * 30f;
            yield return null;
        }
    }
}