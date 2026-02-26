using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : OverworldObject
{
    [Header("Stats")]
    public float moveSpeed = 5;
    public float jumpForce = 8;
    public float gravity = -20;
    public float maxHP = 2;
    public float currentHP = 2;
    
    [SerializeField] private float rotationSpeed = 10;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerModeManager modeManager;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visual")]
    /*[SerializeField] private Transform modelRoot;
    [SerializeField] private Animator defaultAnimator;
    private Animator animator;

    private GameObject currentModelInstance;
    private GameObject defaultModel;
    private RuntimeAnimatorController defaultAnimatorController;*/

    [Header("Air Control")]
    public float airAcceleration = 20f;
    public float gravityAcceleration = 25f;
    public float maxFallSpeed = 25f;

    [Header("Wall")]
    public float wallCheckDistance = 0.7f;
    public LayerMask wallLayer;
    public float wallJumpBackForce = 5f;

    [Header("Combat")]
    [SerializeField] private Damageable damageable;

    [HideInInspector] public DamageInfo damaged;
    [HideInInspector] public bool hasDoubleJumped;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector3 velocity;
    private Vector3 currentWallNormal;
    public bool hasTakenDamageThisFrame = false;

    [SerializeField] GameObject hitboxVisual;
    public event Action<DamageInfo> OnDamaged;

    public Vector2 MoveInput { get; set; }
    public bool JumpPressed { get; set; }
    public bool AttackPressed { get; set; }

    private void Awake()
    {
        if (characterController == null)
        { characterController = GetComponent<CharacterController>(); }

        if (modeManager == null)
        { modeManager = GetComponent<PlayerModeManager>(); }

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        /*defaultModel = modelRoot.GetChild(0).gameObject;
        animator = defaultAnimator;
        defaultAnimatorController = animator.runtimeAnimatorController;*/

        if (damageable != null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += OnDamageReceived;

        if (damageable == null)
        { damageable = GetComponent<Damageable>(); }
        damageable.OnDamaged += TakeDamage;

        // Inicializar HP
        currentHP = maxHP;

        damaged = new DamageInfo(0, null);
    }

    private void OnDestroy()
    {
        if (damageable != null)
            damageable.OnDamaged -= OnDamageReceived;
    }

    public void OnDamageReceived(DamageInfo info)
    {
        OnDamaged?.Invoke(info);
        damaged = new DamageInfo(info.amount, info.source);
    }

    // -- Animator --
    private void LateUpdate()
    {
        if (!isPaused/* && animator != null*/)
        {
            float newMoveSpeed = (MoveInput * moveSpeed).magnitude;
            //animator.SetFloat("Speed", newMoveSpeed);
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);
            //animator.SetBool("IsGrounded", isGrounded);
        }
        hasTakenDamageThisFrame = false;
    }

    public void AnimTrigger(string Name)
    {
        /*if (animator != null)
        {
            //animator.SetTrigger(Name);
        }*/
    }

    public void AnimBool(string Name, bool state)
    {
        /*if (animator != null)
            animator.SetBool(Name, state);*/
    }

    public void ApplyVisualOverride(PlayerModeData data)
    {
        if (data.modelPrefab != null)
        {
            /*if (currentModelInstance != null)
                Destroy(currentModelInstance);

            defaultModel.SetActive(false);

            currentModelInstance = Instantiate(data.modelPrefab, modelRoot);

            animator = currentModelInstance.GetComponent<Animator>();*/
        }
    }
    public void RestoreDefaultVisual()
    {
        /*if (currentModelInstance != null)
            Destroy(currentModelInstance);

        defaultModel.SetActive(true);
        animator.runtimeAnimatorController = defaultAnimatorController;
        animator = defaultAnimator;*/
    }

    // -- Funciones de control --
    public void Move()
    {
        if (MoveInput == Vector2.zero)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
            velocity.z = Mathf.Lerp(velocity.z, 0, 0.5f);
            return;
        }

        float finalSpeed = modeManager.ModifyMoveSpeed(moveSpeed);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        if (isGrounded)
        {
            Vector3 moveDirection = camForward * MoveInput.y + camRight * MoveInput.x;

            characterController.Move(moveDirection * finalSpeed * Time.deltaTime);
            RotateTowards(moveDirection);

            velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
            velocity.z = Mathf.Lerp(velocity.z, 0, 0.5f);
        }
        else
        {
            Vector3 airMoveDirection = new Vector3(velocity.x, 0, velocity.z);
            airMoveDirection = Vector3.Lerp(airMoveDirection, (camForward * MoveInput.y + camRight * MoveInput.x), 5f);

            characterController.Move(airMoveDirection * finalSpeed * Time.deltaTime);
            RotateTowards(airMoveDirection);

            velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
            velocity.z = Mathf.Lerp(velocity.z, 0, 0.5f);
        }
    }

    private void RotateTowards(Vector3 direction)
    {
        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public bool CheckWall(out RaycastHit hit)
    {
        hit = new RaycastHit();
        Vector3 origin = transform.position + Vector3.up * 1f;

        if (Physics.SphereCast(
            origin,
            0.3f,
            transform.forward,
            out RaycastHit frontHit,
            wallCheckDistance,
            wallLayer))
        {
            float angle = Vector3.Angle(Vector3.up, frontHit.normal);
            if (angle > 45)
            {
                hit = frontHit;
                currentWallNormal = frontHit.normal;
                Debug.Log($"Pared frontal: {frontHit.collider.name}, ángulo: {angle:F1}°");
                return true;
            }
        }

        return false;
    }

    public void ApplyGravity(float gravity = 1)
    {
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        else
        {
            velocity.y -= gravityAcceleration * Time.deltaTime * gravity;

            if (velocity.y < -maxFallSpeed)
                velocity.y = -maxFallSpeed;
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    public void ResetFallSpeed()
    { velocity.y = 0; }

    public void Jump()
    {
        float finalJump = modeManager.ModifyJumpForce(jumpForce);
        velocity.y = finalJump;
    }

    public void ClimbMove()
    {
        RotateTowards(-currentWallNormal);

        if (MoveInput == Vector2.zero)
            return;

        float finalSpeed = modeManager.ModifyMoveSpeed(moveSpeed);

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 desiredDirection =
            Vector3.up * MoveInput.y +
            camRight * MoveInput.x;

        Vector3 wallMoveDirection =
            Vector3.ProjectOnPlane(desiredDirection, currentWallNormal).normalized;

        characterController.Move(wallMoveDirection * finalSpeed * Time.deltaTime);
    }

    public void TakeDamage(DamageInfo info)
    {
        if (hasTakenDamageThisFrame) return;
        hasTakenDamageThisFrame = true;



        //Knockback
        if (damaged.source != null)
        {
            // Hacer el daño
            currentHP -= damaged.amount;
            Debug.Log("Player damaged " + currentHP);
            Vector3 damageDir = (transform.position - damaged.source.transform.position).normalized;
            if (!damaged.source.gameObject.CompareTag("Bullet")) Knockback(damageDir);

            if (currentHP <= 0) GameSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().name, SceneTransition.FadeBlack, false);
        }
    }

    //-- Pausa --
    protected override void OnGamePaused()
    {
        //animator.speed = 0f;
    }
    protected override void OnGameResumed()
    {
        //animator.speed = 1;
    }


    // Movemos al jugador en la dirección del golpe para evitar softlocks
    public void Knockback(Vector3 dir)
    {
        float knockbackSpeed = 25;
        StartCoroutine(IEKnockback(dir, knockbackSpeed));
    }

    private IEnumerator IEKnockback(Vector3 dir, float knockbackSpeed)
    {
        while (knockbackSpeed > 0)
        {
            yield return null;
            characterController.Move(dir * Time.deltaTime * knockbackSpeed);
            knockbackSpeed--;
        }
    }
}