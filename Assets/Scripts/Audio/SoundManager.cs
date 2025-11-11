using UnityEngine;
using System.Collections;

/// <summary>
/// Manages all game sound effects
/// Uses procedurally generated sounds (no external audio files needed)
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Sound Settings")]
    [SerializeField] [Range(0f, 1f)] private float masterVolume = 0.7f;
    [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.8f;

    [Header("Sound Generation Settings")]
    [SerializeField] private int sampleRate = 44100;
    [SerializeField] private float kickFrequency = 200f;
    [SerializeField] private float passFrequency = 300f;
    [SerializeField] private float goalFrequency = 440f;
    [SerializeField] private float whistleFrequency = 2000f;
    [SerializeField] private float skillSoundFrequency = 600f;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Create audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = masterVolume * sfxVolume;
        audioSource.playOnAwake = false;
    }

    /// <summary>
    /// Play kick sound (deep thud)
    /// </summary>
    public void PlayKick()
    {
        PlaySound(kickFrequency, 0.15f, SoundType.Kick);
    }

    /// <summary>
    /// Play pass sound (lighter kick)
    /// </summary>
    public void PlayPass()
    {
        PlaySound(passFrequency, 0.1f, SoundType.Pass);
    }

    /// <summary>
    /// Play goal celebration sound
    /// </summary>
    public void PlayGoal()
    {
        StartCoroutine(PlayGoalSequence());
    }

    /// <summary>
    /// Play whistle sound (match start/end)
    /// </summary>
    public void PlayWhistle()
    {
        PlaySound(whistleFrequency, 0.3f, SoundType.Whistle);
    }

    /// <summary>
    /// Play bounce sound (ball hitting wall)
    /// </summary>
    public void PlayBounce()
    {
        PlaySound(400f, 0.05f, SoundType.Bounce);
    }

    /// <summary>
    /// Play skill activation sound
    /// </summary>
    public void PlaySkillSound(SkillType skill)
    {
        switch (skill)
        {
            case SkillType.SpeedBoost:
                PlaySound(800f, 0.3f, SoundType.SkillActivation);
                break;
            case SkillType.PowerShot:
                PlaySound(150f, 0.4f, SoundType.PowerShot);
                break;
            case SkillType.CurveShot:
                PlaySound(500f, 0.25f, SoundType.SkillActivation);
                break;
            case SkillType.SkillMove:
                PlaySound(1000f, 0.15f, SoundType.SkillActivation);
                break;
            case SkillType.SlideTackle:
                PlaySound(250f, 0.3f, SoundType.SlideTackle);
                break;
            case SkillType.ShieldBall:
                PlaySound(350f, 0.2f, SoundType.SkillActivation);
                break;
        }
    }

    private void PlaySound(float frequency, float duration, SoundType type)
    {
        AudioClip clip = GenerateSound(frequency, duration, type);
        audioSource.PlayOneShot(clip, masterVolume * sfxVolume);
    }

    private IEnumerator PlayGoalSequence()
    {
        // Goal celebration: rising tone sequence
        for (int i = 0; i < 3; i++)
        {
            float freq = goalFrequency * (1f + i * 0.2f);
            AudioClip clip = GenerateSound(freq, 0.2f, SoundType.Goal);
            audioSource.PlayOneShot(clip, masterVolume * sfxVolume);
            yield return new WaitForSeconds(0.15f);
        }
    }

    private AudioClip GenerateSound(float frequency, float duration, SoundType type)
    {
        int sampleCount = Mathf.FloorToInt(sampleRate * duration);
        AudioClip clip = AudioClip.Create("GeneratedSound", sampleCount, 1, sampleRate, false);

        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;
            float envelope = GetEnvelope(t, duration, type);
            float wave = GetWaveform(t, frequency, type);

            samples[i] = wave * envelope;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    private float GetWaveform(float time, float frequency, SoundType type)
    {
        float phase = time * frequency * 2f * Mathf.PI;

        switch (type)
        {
            case SoundType.Kick:
            case SoundType.Pass:
                // Deep bass thump with noise
                float noise = Random.Range(-0.3f, 0.3f);
                return Mathf.Sin(phase) * 0.7f + noise * 0.3f;

            case SoundType.Goal:
                // Bright tone
                return Mathf.Sin(phase);

            case SoundType.Whistle:
                // Pure tone
                return Mathf.Sin(phase) * 0.8f;

            case SoundType.Bounce:
                // Short click
                return Mathf.Sin(phase) * 0.5f + Random.Range(-0.2f, 0.2f);

            case SoundType.SkillActivation:
                // Rising electronic sound
                return Mathf.Sin(phase * (1f + time)) * 0.7f;

            case SoundType.PowerShot:
                // Deep explosive sound
                return Mathf.Sin(phase * 0.5f) * 0.9f + Random.Range(-0.4f, 0.4f);

            case SoundType.SlideTackle:
                // Scraping sound
                return (Random.Range(-0.5f, 0.5f) * 0.7f) + (Mathf.Sin(phase) * 0.3f);

            default:
                return Mathf.Sin(phase);
        }
    }

    private float GetEnvelope(float time, float duration, SoundType type)
    {
        float t = time / duration;

        switch (type)
        {
            case SoundType.Kick:
            case SoundType.Pass:
                // Fast attack, medium decay
                return Mathf.Exp(-t * 8f);

            case SoundType.Goal:
                // Smooth attack and decay
                return Mathf.Sin(t * Mathf.PI);

            case SoundType.Whistle:
                // Sustained with slight decay
                return 1f - (t * 0.3f);

            case SoundType.Bounce:
                // Very fast decay
                return Mathf.Exp(-t * 15f);

            case SoundType.SkillActivation:
                // Rise and fall
                return Mathf.Sin(t * Mathf.PI) * 0.8f;

            case SoundType.PowerShot:
                // Explosive with decay
                return Mathf.Exp(-t * 5f);

            case SoundType.SlideTackle:
                // Linear decay
                return 1f - t;

            default:
                return 1f - t;
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = masterVolume * sfxVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        if (audioSource != null)
        {
            audioSource.volume = masterVolume * sfxVolume;
        }
    }
}

public enum SoundType
{
    Kick,
    Pass,
    Goal,
    Whistle,
    Bounce,
    SkillActivation,
    PowerShot,
    SlideTackle
}
