using UnityEngine;

public class PlayerStateMachine : OverworldObject
{
    public PlayerState currentState;
    bool active;

    private void Start()
    {
        Initialize(new IdleState(this, GetComponent<PlayerController>()));
    }

    public void Initialize(PlayerState startingState)
    {
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(PlayerState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    private void Update()
    {
        if (!isPaused)
        {
            currentState?.HandleInput();
            currentState?.Update();
        }
    }
}
