using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : OverworldObject
{
    [Header("Stuff")]
    [SerializeField] private PlayerMovement movement;
    [SerializeField] private Animator animator;
    [SerializeField, Range(0,1)] private float animationPauseSpeed = 0.1f;

    [Header("Bendition")]
    [SerializeField] public Player_SO bendition;

    private InputSystem_Actions input;
    private bool hasControl = true;

    private bool onDialogue;

    private void Awake()
    {
        if (movement == null)
            movement = GetComponent<PlayerMovement>();

        onDialogue = false;

        input = new InputSystem_Actions();

        input.Player.Move.performed += ctx => movement.SetMoveInput(ctx.ReadValue<Vector2>());
        input.Player.Move.canceled += ctx => movement.SetMoveInput(Vector2.zero);
        input.Player.Jump.started += ctx => movement.BufferJump();
        input.Player.Interact.started += ctx => OnInteract();
        input.Player.Pause.started += ctx => OnPause();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        input.Enable();

        DialogueManager.OnDialogueStarted += OnDialogueStarted;
        DialogueManager.OnDialogueFinished += OnDialogueFinished;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        input.Disable();

        DialogueManager.OnDialogueStarted -= OnDialogueStarted;
        DialogueManager.OnDialogueFinished -= OnDialogueFinished;
    }

    /* Update Control */

    private void Update()
    {
        if (!hasControl || GameManager.Instance.IsPaused)
            return;

        movement.HandleUpdate();
    }

    private void OnInteract()
    {
        if (onDialogue)
        {
            if (DialogueManager.Instance != null)
            { DialogueManager.Instance.NextLine(); }
        }
    }

    private void OnPause()
    {
        if (GameManager.Instance.IsPaused)
        { return; }

        MenuManager.Instance.OpenPauseMenu();
        GameManager.Instance.PauseGame();
    }

    /* Dailogue functions */

    void OnDialogueStarted()
    {
        movement.SetMoveInput(Vector2.zero);
        DisableControl();
        onDialogue = true;
    }

    void OnDialogueFinished()
    {
        EnableControl();
        onDialogue = false;
    }

    /* API Control */

    public void EnableControl()
    {
        animator.speed = 1;
        movement.StopMovement();
        hasControl = true;
    }

    public void DisableControl()
    {
        hasControl = false;
        movement.StopMovement();
    }

    protected override void OnGamePaused()
    { 
        hasControl = false;
        animator.speed = animationPauseSpeed;
    }
    protected override void OnGameResumed()
    {
        animator.speed = 1;
        movement.StopMovement();
        hasControl = true;
    }
}
