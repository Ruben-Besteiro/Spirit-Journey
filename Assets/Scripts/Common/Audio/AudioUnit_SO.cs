using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Audio Unit")]
public class AudioUnit_SO : ScriptableObject
{
    [Header("Clips")]
    public AudioClip[] clips;

    [Header("Settings")]
    [Range(0f, 1f)] public float volume = 1f;
    public float minDistance = 1f;
    public float maxDistance = 25f;

    public AudioClip GetRandomClip()
    {
        if (clips == null || clips.Length == 0)
            return null;

        if (clips.Length == 1)
            return clips[0];

        return clips[Random.Range(0, clips.Length)];
    }
}
