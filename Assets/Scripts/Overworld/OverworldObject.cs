using UnityEngine;

public abstract class OverworldObject : MonoBehaviour
{
    protected bool isPaused;

    protected virtual void OnEnable()
    {
        GameManager.OnGamePaused += HandlePauseEnabled;
        GameManager.OnGameResumed += HandlePauseDisabled;
        isPaused = GameManager.Instance.IsPaused;
    }

    protected virtual void OnDisable()
    {
        GameManager.OnGamePaused -= HandlePauseEnabled;
        GameManager.OnGameResumed -= HandlePauseDisabled;
    }

    private void HandlePauseEnabled()
    {
        isPaused = true;
        OnGamePaused();
    }

    private void HandlePauseDisabled()
    {
        isPaused = false;
        OnGameResumed();
    }

    /// <summary>
    /// Se llama automáticamente cuando el juego entra en pausa
    /// </summary>
    protected virtual void OnGamePaused() { }

    /// <summary>
    /// Se llama automáticamente cuando el juego sale de pausa
    /// </summary>
    protected virtual void OnGameResumed() { }
}
