using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : OverworldObject
{
    [SerializeField] GameObject[] drops;      // La lista de posibles objetos que el enemigo puede dropear

    [Header("Movement")]
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected bool startFacingRight = true;
    public bool canMove = true;
    Vector3 vectorToPlayer;
    protected Coroutine attackCoroutine;

    [Header("Animation")]
    [SerializeField] protected Animator animator;

    [Header("Detection")]
    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected float forwardCheckDistance = 0.2f;
    [SerializeField] protected float edgeCheckDistance = 0.4f;
    [SerializeField] protected float skinWidth = 0.02f;
    [SerializeField] protected float playerDetectDistance = 20;
    [SerializeField] protected BoxCollider damageBox;

    [Header("Health")]
    [SerializeField] protected int maxHealth = 2;

    [Header("Debug")]
    [SerializeField] private bool drawGizmos = true;

    protected Transform playerTransform;
    protected CharacterController controller;
    protected EnemyStates currentState = EnemyStates.Wait;
    protected int currentHealth;
    protected Vector3 velocity;
    protected bool grounded;
    protected bool groundedPrev;
    RaycastHit hit;
    NavMeshAgent agent;

    void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isPaused)
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
            case EnemyStates.Attack:
                AttackCooldownUpdate();
                break;
        }
    }

    protected virtual void WaitUpdate()
    {
        Debug.Log("Enemigo esperando para atacar");

        Debug.DrawRay(transform.position, vectorToPlayer * playerDetectDistance, Color.red);
        if (Physics.Raycast(transform.position, vectorToPlayer, out hit, playerDetectDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hit Player!");
                currentState = EnemyStates.Chase;
            }
            else
            {
                Debug.Log($"Hit: {hit.collider.name}");
            }
        }
        else
        {
            Debug.Log("No hit");
        }
    }


    protected virtual void ChaseUpdate()
    {
        // Nos persigue usando el Nav Mesh
        agent.isStopped = false;
        agent.SetDestination(playerTransform.position);

        if (vectorToPlayer.magnitude <= agent.stoppingDistance + .1f)
        {
            print("Jugador encontrado");
            currentState = EnemyStates.Attack;
        }

        Debug.DrawRay(transform.position, vectorToPlayer * hit.distance, Color.red);
        if (Physics.Raycast(transform.position, vectorToPlayer, out hit, playerDetectDistance))
        {
            if (!hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.CompareTag("Enemy"))
            {
                print("Did Not Hit Player");
                agent.isStopped = true;
                currentState = EnemyStates.Wait;
            }
        }
    }

    // Esto se llama cuando el Damage Box detecta algo (el enemigo no tiene más triggers)
    protected virtual void OnTriggerEnter(Collider other)
    {
        print("Le diste a algo");
        Damageable d = other.gameObject.GetComponent<Damageable>();
        // El enemigo debe estar en estado Chase para hacer daño porque está "cargando" hacia nosotros
        if (d == null || currentState != EnemyStates.Chase || other.gameObject.CompareTag("Enemy")) return;

        d.ApplyDamage(vectorToPlayer.normalized, 1);
        currentState = EnemyStates.Attack;
    }

    protected virtual void AttackCooldownUpdate()
    {
        if (attackCoroutine != null) return;
        attackCoroutine = StartCoroutine(IEAttackCooldown());
    }

    protected IEnumerator IEAttackCooldown()
    {
        agent.isStopped = true;
        damageBox.enabled = false;
        yield return new WaitForSeconds(1);
        attackCoroutine = null;
        currentState = EnemyStates.Chase;
        damageBox.enabled = true;
        agent.isStopped = false;
    }

    protected void TakeDamage()
    {

    }
}
