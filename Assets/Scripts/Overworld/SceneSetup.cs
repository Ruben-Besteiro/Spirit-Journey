using UnityEngine;

public class SceneSetup : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        { GameManager.Instance.ResumeGame(); }
    }
}
