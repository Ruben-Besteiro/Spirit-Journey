using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float gravity = -20f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("References")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private PlayerModeManager modeManager;


    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.3f;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 velocity;
    private bool isGrounded;

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
    }

    private void LateUpdate()
    {
        //Animator
        float newMoveSpeed = (MoveInput * moveSpeed).magnitude;
        animator.SetFloat("Speed", newMoveSpeed);
        isGrounded = Physics.CheckSphere( groundCheck.position, groundDistance, groundLayer );
        animator.SetBool("IsGrounded", isGrounded);
    }

    public void AnimTrigger(string Name)
    { animator.SetTrigger(Name); }

    public void Move()
    {
        if (MoveInput == Vector2.zero)
            return;

        float finalSpeed = modeManager.ModifyMoveSpeed(moveSpeed);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 moveDirection = camForward * MoveInput.y + camRight * MoveInput.x;

        characterController.Move(moveDirection * finalSpeed * Time.deltaTime);

        RotateTowards(moveDirection);
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

    public void ApplyGravity()
    {
        isGrounded = characterController.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    public void Jump()
    {
        if (characterController.isGrounded)
        {
            float finalJump = modeManager.ModifyJumpForce(jumpForce);
            velocity.y = finalJump;
        }
    }

}
