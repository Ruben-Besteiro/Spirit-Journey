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
            // El raycast coge el primer objeto que no sea del propio enemigo
            if (h.collider.transform.root == transform.root) continue;

            hit = h;

            if (hit.collider.CompareTag("Player") || hit.collider.gameObject.layer == 3)        // 
            {
                Debug.Log("Jugador encontrado");
                Debug.DrawRay(transform.position, vectorToPlayer.normalized * playerDetectDistance, Color.green);
                currentState = EnemyStates.Chase;
            }
            else
            {
                if (hit.collider.gameObject.layer == 3) print("Detectó el suelo erróneamente");
                print(gameObject.name + " Se encontró " + hit.collider.gameObject.name);
                Debug.DrawRay(transform.position, vectorToPlayer.normalized * playerDetectDistance, Color.red);
            }

            break;
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
            return;
        }

        RaycastHit[] hits = Physics.RaycastAll(transform.position, vectorToPlayer.normalized, playerDetectDistance);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        foreach (RaycastHit h in hits)
        {
            if (h.collider.transform.root == transform.root) continue;

            Debug.DrawRay(transform.position, vectorToPlayer.normalized * h.distance, Color.red);

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
            if (!other.gameObject.GetComponent<PlayerController>().hasTakenDamageThisFrame)
                other.gameObject.GetComponent<Damageable>().TakeDamage(new DamageInfo(1, gameObject));
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
        //agent.SetDestination(playerTransform.position);
    }
}