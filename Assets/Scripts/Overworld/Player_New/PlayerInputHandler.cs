using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [SerializeField] private PlayerController controller;
    private InputSystem_Actions input;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        
        input = new InputSystem_Actions();

        Debug.Log(input);

        input.Player.Move.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        input.Player.Move.canceled += ctx => OnMove(Vector2.zero);
        input.Player.Jump.started += ctx => OnJump(ctx);
        input.Player.Jump.canceled += ctx => OnJump(ctx);
        input.Player.Attack.started += ctx => OnAttack(ctx);
        input.Player.Attack.canceled += ctx => OnAttack(ctx);
    }
    private void OnEnable()
    {
        input.Player.Enable();
    }

    private void OnDisable()
    {
        input.Player.Disable();
    }

    public void OnMove(Vector2 movement)
    {
        controller.MoveInput = movement;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            controller.JumpPressed = true;

        if (context.canceled)
            controller.JumpPressed = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
            controller.AttackPressed = true;

        if (context.canceled)
            controller.AttackPressed = false;
    }
}
