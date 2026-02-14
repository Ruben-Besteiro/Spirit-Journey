using UnityEngine;

public class dt2 : MonoBehaviour
{
        [SerializeField] private DialogueData_SO dialogue;
        [SerializeField] private bool oneShot = true;

        private bool hasTriggered;

        private void OnTriggerEnter(Collider other)
        {
            if (hasTriggered)
                return;

            if (!other.CompareTag("Enemy"))
                return;

            DialogueManager.Instance.StartDialogue(dialogue);

            if (oneShot)
            {
                hasTriggered = true;
                gameObject.SetActive(false);
            }
        }
}
