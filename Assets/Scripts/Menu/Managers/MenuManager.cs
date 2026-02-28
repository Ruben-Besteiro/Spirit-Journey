using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private CinemachineInputAxisController inputController;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private float enterDuration = 0.35f;
    [SerializeField] private float enterOffset = 700f;

    private RectTransform panelRect;
    private Vector2 panelFinalPos;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        panelRect = pausePanel.GetComponent<RectTransform>();
        panelFinalPos = panelRect.anchoredPosition;

        pausePanel.SetActive(false);
        GameManager.Instance.ResumeGame();
        LockCursor();
    }

    private void UnlockCursor()
    {
        inputController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void LockCursor()
    {
        inputController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void OpenPauseMenu()
    {
        if (GameManager.Instance.IsPaused)
            return;

        UnlockCursor();

        pausePanel.SetActive(true);

        panelRect.anchoredPosition =
            panelFinalPos + Vector2.left * enterOffset;

        panelRect
            .DOAnchorPos(panelFinalPos, enterDuration)
            .SetEase(Ease.OutCubic);
    }

    public void ResumeGame()
    {
        panelRect
            .DOAnchorPos(panelFinalPos + Vector2.left * enterOffset, enterDuration)
            .SetEase(Ease.InCubic)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                pausePanel.SetActive(false);
                LockCursor();
                GameManager.Instance.ResumeGame();
            });
    }
}



