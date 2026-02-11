using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed = 6f;
    [SerializeField] public float jumpHeight = 1.8f;
    [SerializeField] protected float gravity = -9.81f;
    [SerializeField] protected float rotationSpeed = 12f;

    [Header("References")]
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected Transform cameraTransform;
    [SerializeField] protected Animator animator;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Helpers")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    protected Vector3 velocity;
    private bool isGrounded;

    protected float coyoteTimer;
    protected float jumpBufferTimer;

    protected Vector2 moveInput;
    protected Vector3 moveDir;

    // Cosas del lagarto
    protected Vector3 gravityDirection = new Vector3(0, -1, 0);
    protected bool isTouchingWallThisFrame = false;
    protected bool wasOnWallLastFrame = false;
    protected Coroutine wallExitCoroutine;
    protected Vector3 wallNormal;

    PlayerActions pa;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        pa = GetComponent<PlayerActions>();
    }

    private void Start()
    {
        if (pa.playerClass == Classes.Lizard)
        {
            controller.slopeLimit = 90;
            controller.minMoveDistance = 0;
            print("El jugador es un lagarto");
        }
    }

    /* Public Input */

    public void SetMoveInput(Vector2 input)
    {
        moveInput = input;
    }

    public void BufferJump()
    {
        jumpBufferTimer = jumpBufferTime;
    }

    public void HandleUpdate()
    {
        CheckGround();
        UpdateTimers();
        HandleMovement();
        HandleJump();
        ApplyGravity();
        UpdateAnimator();
        LateUpdate();       // Esto es necesario por que por alguna razón si no lo pongo el LateUpdate no se llama
    }

    /* Movement */

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );

        if (isGrounded || transform.up != Vector3.up)
        {
            coyoteTimer = coyoteTime;

            if (velocity.y < 0f)
                velocity.y = -2f;

            if (!isTouchingWallThisFrame)
            {
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }
    }

    private void UpdateTimers()
    {
        coyoteTimer -= Time.deltaTime;
        jumpBufferTimer -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;  // Eliminamos la Y para evitar cosas raras
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;  // Ver arriba
        camRight.Normalize();

        Vector3 moveDir;
        Vector3 playerUp = transform.up;

        bool isOnASlopeOrGround = Mathf.Abs(playerUp.y) > 0.25f;

        if (isOnASlopeOrGround)     // Movimiento normal
        {
            Vector3 wallForward = Vector3.ProjectOnPlane(camForward, playerUp).normalized;
            Vector3 wallRight = Vector3.ProjectOnPlane(camRight, playerUp).normalized;

            moveDir = wallForward * moveInput.y + wallRight * moveInput.x;
        }
        else  // Movimiento en pared
        {
            Vector3 wallRight = Vector3.Cross(wallNormal, Vector3.ProjectOnPlane(Vector3.up, wallNormal)).normalized;
            moveDir = (wallRight * moveInput.x + Vector3.up * moveInput.y).normalized;
        }

        controller.Move(moveDir * moveSpeed * Time.deltaTime);
        RotateTowardsMovement(moveDir);
    }

    private void RotateTowardsMovement(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f) return;

        Vector3 projectedDirection = Vector3.ProjectOnPlane(direction, transform.up);

        if (projectedDirection.sqrMagnitude < 0.001f) return;

        Quaternion lookRotation = Quaternion.LookRotation(projectedDirection, transform.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            gravityDirection = Vector3.down;
            transform.rotation = Quaternion.Euler(Vector3.zero);

            velocity = wallNormal * moveSpeed * .5f; // Impulso horizontal
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Impulso vertical
            animator.SetTrigger("Jump");
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            animator.SetTrigger("Jump");
        }
    }

    private void ApplyGravity()
    {
        velocity += gravityDirection * Mathf.Abs(gravity) * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        float speed = moveInput.magnitude;

        animator.SetFloat("Speed", speed, 0.1f, Time.deltaTime);
        animator.SetBool("IsGrounded", isGrounded);
    }

    internal void StopMovement()
    {
        velocity = Vector3.zero;
        jumpBufferTimer = 0f;
        moveInput = Vector3.zero;
        animator.SetFloat("Speed", 0);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Si somos un lagarto y tocamos una pared, nos pegamos a ella
        if (pa.playerClass == Classes.Lizard)
        {
            if (hit.gameObject.CompareTag("ScalableWall"))
            {
                isTouchingWallThisFrame = true;
                wallNormal = hit.normal;
                isGrounded = true;

                // Cambiamos la gravedad y la rotación del modelo
                gravityDirection = -wallNormal;
                transform.rotation = Quaternion.FromToRotation(transform.up, wallNormal) * transform.rotation;

                velocity = Vector3.zero;
            }
        }
    }

    private void LateUpdate()
    {
        // Esto lo utiliza el lagarto para ver si se despegó de una pared
        if (pa.playerClass == Classes.Lizard)
        {
            if (wasOnWallLastFrame && !isTouchingWallThisFrame)
            {
                if (wallExitCoroutine != null)
                {
                    StopCoroutine(wallExitCoroutine);
                }
                // Aplicamos un retardo para evitar los falsos positivos
                wallExitCoroutine = StartCoroutine(CheckWallExit());
            }

            if (isTouchingWallThisFrame && wallExitCoroutine != null)
            {
                StopCoroutine(wallExitCoroutine);
                wallExitCoroutine = null;
            }
            wasOnWallLastFrame = isTouchingWallThisFrame;
            isTouchingWallThisFrame = false;
        }
    }

    IEnumerator CheckWallExit()
    {
        yield return new WaitForSeconds(0.25f);
        if (!isTouchingWallThisFrame)
        {
            // Reseteamos la gravedad y la rotación del modelo
            gravityDirection = Vector3.down;
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
        }
    }
}