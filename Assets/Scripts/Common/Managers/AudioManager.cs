using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioDatabase audioDatabase;

    [Header("Spatial Audio")]
    [SerializeField] private Transform spatialRoot;
    [SerializeField] private AudioMixer audioMixer;

    private const string MUSIC_VOL = "MusicVolume";
    private const string SFX_VOL = "SFXVolume";
    private const string VOICE_VOL = "VoiceVolume";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public float GetMusicVolume() => DbToLinear(MUSIC_VOL);
    public float GetSFXVolume() => DbToLinear(SFX_VOL);
    public float GetVoiceVolume() => DbToLinear(VOICE_VOL);

    private float DbToLinear(string param)
    {
        audioMixer.GetFloat(param, out float db);
        return Mathf.Pow(10f, db / 20f);
    }

    /* MUSIC */
    public void PlayMusic(string category, string unit)
    {
        AudioUnit_SO audioUnit = audioDatabase.GetUnit(category, unit);
        if (audioUnit == null) { return; }

        PlayMusic(audioUnit);
    }
    public void PlayMusic(AudioUnit_SO unit)
    {
        if (unit == null) { return; }

        AudioClip clip = unit.GetRandomClip();

        if (musicSource.clip == clip) { return; }

        musicSource.Stop();
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    /* SFX 3D */
    public void PlaySFX3D(string category, string unit, Vector3 position)
    {
        AudioUnit_SO audioUnit = audioDatabase.GetUnit(category, unit);
        if (audioUnit == null) { return; }

        PlaySFX3D(audioUnit, position);
    }

    public void PlaySFX3D(string category, int index, Vector3 position)
    {
        AudioUnit_SO audioUnit = audioDatabase.GetUnit(category, index);
        if (audioUnit == null) { return; }

        PlaySFX3D(audioUnit, position);
    }

    public void PlaySFX3D(AudioUnit_SO unit, Vector3 position)
    {
        if (unit == null) { return; }

        AudioClip clip = unit.GetRandomClip();
        if (clip == null) { return; }

        AudioSource source = CreateSpatialSource(position);
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        source.clip = clip;
        source.volume = unit.volume;
        source.minDistance = unit.minDistance;
        source.maxDistance = unit.maxDistance;
        source.Play();

        Destroy(source.gameObject, clip.length);
    }


    /* VOICE 3D */
    public void PlayVoice3D(string category, string unit, Vector3 position)
    {
        AudioUnit_SO audioUnit = audioDatabase.GetUnit(category, unit);
        if (audioUnit == null) { return; }

        PlayVoice3D(audioUnit, position);
    }

    public void PlayVoice3D(string category, int index, Vector3 position)
    {
        AudioUnit_SO audioUnit = audioDatabase.GetUnit(category, index);
        if (audioUnit == null) { return; }

        PlayVoice3D(audioUnit, position);
    }

    public void PlayVoice3D(AudioUnit_SO unit, Vector3 position)
    {
        if (unit == null) { return; }

        AudioClip clip = unit.GetRandomClip();
        if (clip == null) { return; }

        AudioSource source = CreateSpatialSource(position);
        source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("Voice")[0];
        source.clip = clip;
        source.volume = unit.volume;
        source.minDistance = unit.minDistance;
        source.maxDistance = unit.maxDistance;
        source.Play();

        Destroy(source.gameObject, clip.length);
    }

    /* VOLUMEN */

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat(MUSIC_VOL, LinearToDb(value));
    }

    public void SetSFXVolume(float value)
    {
        audioMixer.SetFloat(SFX_VOL, LinearToDb(value));
    }

    public void SetVoiceVolume(float value)
    {
        audioMixer.SetFloat(VOICE_VOL, LinearToDb(value));
    }

    /* HELPERS */

    private AudioSource CreateSpatialSource(Vector3 position)
    {
        GameObject go = new GameObject("SpatialAudio");
        go.transform.position = position;
        go.transform.parent = spatialRoot;

        AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.dopplerLevel = 0f;
        source.minDistance = 1f;
        source.maxDistance = 25f;

        return source;
    }

    private float LinearToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
