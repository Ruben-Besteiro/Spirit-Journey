using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine stateMachine;
    [SerializeField] private PlayerController controller;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        controller = GetComponent<PlayerController>();
    }

    private void Start()
    {
        stateMachine.Initialize(new IdleState(stateMachine, controller));
    }
}
