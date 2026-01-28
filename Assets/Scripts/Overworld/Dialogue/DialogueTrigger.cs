using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData_SO dialogue;
    [SerializeField] private bool oneShot = true;

    private bool hasTriggered;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered)
            return;

        if (!other.CompareTag("Player"))
            return;

        DialogueManager.Instance.StartDialogue(dialogue);

        if (oneShot)
        {
            hasTriggered = true;
            gameObject.SetActive(false);
        }
    }
}
