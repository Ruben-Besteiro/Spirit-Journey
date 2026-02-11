using System.Collections;
using System.Collections.Generic;
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
    public Classes playerClass = Classes.Default;
    const float MAXTRANSFORMTIMER = 99999;      // El tiempo que duran las transformaciones
    float currentTransformCooldown = 2;     // Cuando nos destransformamos cuenta hacia delante y solo nos podremos transformar si es igual a maxTC
    const float MAXTRANSFORMCOOLDOWN = 2;   
    bool transformed = false;
    private Coroutine transformCoroutine;

    List<Player_SO> benditionList = new List<Player_SO>();
    int currentBenditionIndex;

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
        input.Player.SecondaryAttack.started += ctx => SecondaryAttack();
        input.Player.Interact.started += ctx => OnInteract();
        input.Player.Pause.started += ctx => OnPause();
        input.Player.Transform.started += ctx => OnTransform();
        input.Player.ChangeBendition.started += ctx => OnChangeBendition();

        benditionList.Add(Resources.Load<Player_SO>("Fox SO"));
        benditionList.Add(Resources.Load<Player_SO>("Lizard SO"));
    }

    private void Start()
    {
        // El jugador empieza con el zorro desbloqueado. Pero se tiene que transformar dándole a la tecla de transformarse
        currentBenditionIndex = 1;
        bendition = benditionList[currentBenditionIndex];
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

        if (currentTransformCooldown < MAXTRANSFORMCOOLDOWN && !transformed) currentTransformCooldown += Time.deltaTime;
        else print("Llegó");

        movement.HandleUpdate();
    }

    private void SecondaryAttack()
    {
       // if (movement.playerClass)
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

    private void OnTransform()
    {
        if (transformed) OnDetransform();

        if (currentTransformCooldown >= MAXTRANSFORMCOOLDOWN)
        {
            transformed = true;

            // Reiniciamos la corrutina
            if (transformCoroutine != null)
            {
                StopCoroutine(transformCoroutine);
                transformCoroutine = null;
            }
            transformCoroutine = StartCoroutine(IETransformTimer());

            movement.moveSpeed = bendition.speed;
            movement.jumpHeight = bendition.jumpHeight;


            switch (bendition.benditionName)
            {
                case "Fox":
                    playerClass = Classes.Fox;
                    GameObject.Find("Alpha_Surface").GetComponent<Renderer>().material.color = Color.red;
                    break;
                case "Lizard":
                    playerClass = Classes.Lizard;
                    GameObject.Find("Alpha_Surface").GetComponent<Renderer>().material.color = Color.green;
                    break;
            }
        }
    }

    private IEnumerator IETransformTimer()
    {
        yield return new WaitForSeconds(MAXTRANSFORMTIMER);
        OnDetransform();
        transformCoroutine = null;
    }

    private void OnDetransform()
    {
        transformed = false;
        if (transformCoroutine != null)
        {
            StopCoroutine(transformCoroutine);
            transformCoroutine = null;
        }
        currentTransformCooldown = 0;

        // Devolvemos todos los parámetros a la normalidad (con el scriptable Default)
        Player_SO defaultPlayer = ((Player_SO)Resources.Load("Default SO"));
        movement.moveSpeed = defaultPlayer.speed;
        movement.jumpHeight = defaultPlayer.jumpHeight;
        playerClass = Classes.Default;
        GameObject.Find("Alpha_Surface").GetComponent<Renderer>().material.color = Color.white;
    }

    private void OnChangeBendition()
    {
        // Escogemos la bendición que vaya después en la lista como si fuese una rueda
        currentBenditionIndex = ++currentBenditionIndex % benditionList.Count;
        bendition = benditionList[currentBenditionIndex];
        print("Bendición actual: " + bendition.benditionName);
    }
}