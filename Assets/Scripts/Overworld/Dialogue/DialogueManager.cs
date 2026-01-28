using UnityEngine;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public static event Action OnDialogueStarted;
    public static event Action OnDialogueFinished;

    private List<string> activeLines;

    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text speakerText;

    [Header("Animation")]
    [SerializeField] private float enterDuration = 0.4f;
    [SerializeField] private float enterOffset = 600f;

    private RectTransform panelRect;
    private Vector2 panelFinalPos;


    private Coroutine dialogueRoutine;
    private DialogueData_SO currentDialogue;
    private int lineIndex;
    private bool isTyping;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        panelRect = dialoguePanel.GetComponent<RectTransform>();
        panelFinalPos = panelRect.anchoredPosition;

        dialoguePanel.SetActive(false);
    }

    /* Públic API */

    public void StartDialogue(DialogueData_SO dialogue)
    {
        if (dialogueRoutine != null)
            StopCoroutine(dialogueRoutine);

        currentDialogue = dialogue;
        activeLines = dialogue.GetLinesForCurrentLanguage();
        lineIndex = 0;

        dialogueText.text = "";
        speakerText.text = dialogue.speakerName;

        dialoguePanel.SetActive(true);

        OnDialogueStarted?.Invoke();

        PlayEnterAnimation(() =>
        {
            dialogueRoutine = StartCoroutine(TypeLine());
        });
    }

    private void PlayEnterAnimation(System.Action onComplete)
    {
        panelRect.anchoredPosition =
            panelFinalPos + Vector2.left * enterOffset;

        panelRect
            .DOAnchorPos(panelFinalPos, enterDuration)
            .SetEase(Ease.OutCubic)
            .OnComplete(() => onComplete?.Invoke());
    }


    public void NextLine()
    {
        if (isTyping)
        {
            StopCoroutine(dialogueRoutine);
            dialogueText.text = activeLines[lineIndex];
            isTyping = false;
            return;
        }

        lineIndex++;

        if (lineIndex >= activeLines.Count)
            EndDialogue();
        else
            dialogueRoutine = StartCoroutine(TypeLine());
    }


    /* Corrutines */

    private IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in activeLines[lineIndex])
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(currentDialogue.typeSpeed);
        }

        isTyping = false;
    }


    private void EndDialogue()
    {
        panelRect
            .DOAnchorPos(panelFinalPos + Vector2.left * enterOffset, enterDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                dialoguePanel.SetActive(false);
                currentDialogue = null;
                OnDialogueFinished?.Invoke();
            });
    }

}
