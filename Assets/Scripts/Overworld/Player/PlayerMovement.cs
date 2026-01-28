using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.8f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float rotationSpeed = 12f;

    [Header("References")]
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Jump Helpers")]
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float jumpBufferTime = 0.15f;

    private Vector3 velocity;
    private bool isGrounded;

    private float coyoteTimer;
    private float jumpBufferTimer;

    private Vector2 moveInput;

    private void Awake()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
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
    }

    /* Movement */

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundLayer
        );

        if (isGrounded)
        {
            coyoteTimer = coyoteTime;

            if (velocity.y < 0f)
                velocity.y = -2f;
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
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDir =
            camForward * moveInput.y +
            camRight * moveInput.x;

        controller.Move(moveDir * moveSpeed * Time.deltaTime);

        RotateTowardsMovement(moveDir);
    }

    private void RotateTowardsMovement(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void HandleJump()
    {
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            animator.SetTrigger("Jump");
        }
    }

    private void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
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
}
