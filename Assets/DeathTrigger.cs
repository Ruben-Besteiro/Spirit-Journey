using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().name, SceneTransition.FadeBlack, false);
        }
    }
}
