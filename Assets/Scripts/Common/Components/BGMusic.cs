using UnityEngine;

public class BGMusic : MonoBehaviour
{
    [SerializeField] private AudioUnit_SO music;

    void Start()
    {
        AudioManager.Instance.PlayMusic(music);
    }

}
