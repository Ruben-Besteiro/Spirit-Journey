using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInputHandler>().enabled = false;
            if (other.gameObject.CompareTag("Player")) StartCoroutine(IEGG());
        }
    }

    private IEnumerator IEGG()
    {
        GameManager gm = GameManager.Instance;

        gm.timerEnabled = false;
        print("¡GG! " + gm.timer + " " + gm.kills);
        yield return new WaitForSeconds(2);
        GameSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().name, SceneTransition.FadeBlack, false);
    }
}
