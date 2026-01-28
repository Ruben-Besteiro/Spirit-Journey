using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    private bool isPaused;

    public bool IsPaused => isPaused;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /* PAUSE SYSTEM */
    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        OnGamePaused?.Invoke();
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        OnGameResumed?.Invoke();
    }

    public void TogglePause()
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}

