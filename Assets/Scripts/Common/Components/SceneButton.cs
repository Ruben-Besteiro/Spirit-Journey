using UnityEngine;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private SceneTransition transitionType;
    [SerializeField] private bool loadingIcon;

    public void LoadScene(string LevelName)
    {
        AudioManager.Instance.PlaySFX3D("UIButton", 0, Camera.main.transform.position);
        GameSceneManager.Instance.LoadScene(LevelName, transitionType, loadingIcon);
    }
}
