using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerState currentState;

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
        currentState?.HandleInput();
        currentState?.Update();
    }
}
