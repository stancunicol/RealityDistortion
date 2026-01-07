using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    [Header("Background Music")]
    [Tooltip("Muzica de fundal care se redă continuu în joc")]
    [SerializeField] private AudioClip backgroundMusic;
    
    [Header("Volume Settings")]
    [Tooltip("Volumul general al jocului (0 = mut, 1 = maxim)")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 0.5f;
    
    [Tooltip("Volumul muzicii de fundal (0 = mut, 1 = maxim)")]
    [Range(0f, 1f)]
    [SerializeField] private float musicVolume = 0.7f;
    
    [Tooltip("Volumul efectelor sonore (0 = mut, 1 = maxim)")]
    [Range(0f, 1f)]
    [SerializeField] private float sfxVolume = 1f;
    
    [Header("Settings")]
    [Tooltip("Muzica pornește automat la start?")]
    [SerializeField] private bool playOnStart = true;
    
    [Tooltip("Obiectul persistă între scene?")]
    [SerializeField] private bool dontDestroyOnLoad = true;
    
    private AudioSource musicAudioSource;
    private static GameAudioManager instance;
    
    public static GameAudioManager Instance
    {
        get { return instance; }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        
        if (dontDestroyOnLoad)
        {
            DontDestroyOnLoad(gameObject);
        }
        
        // Configurare AudioSource pentru muzică
        musicAudioSource = gameObject.GetComponent<AudioSource>();
        if (musicAudioSource == null)
        {
            musicAudioSource = gameObject.AddComponent<AudioSource>();
        }
        
        musicAudioSource.loop = true;
        musicAudioSource.playOnAwake = false;
        musicAudioSource.spatialBlend = 0f; // 2D sound
        
        UpdateVolumes();
    }
    
    void Start()
    {
        if (playOnStart && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }
    
    void OnValidate()
    {
        // Actualizează volumul în timp real în Editor
        if (Application.isPlaying)
        {
            UpdateVolumes();
        }
    }
    
    private void UpdateVolumes()
    {
        if (musicAudioSource != null)
        {
            musicAudioSource.volume = masterVolume * musicVolume;
        }
    }
    
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicAudioSource != null)
        {
            musicAudioSource.clip = backgroundMusic;
            musicAudioSource.Play();
            Debug.Log("[GameAudioManager] Background music started");
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Stop();
            Debug.Log("[GameAudioManager] Background music stopped");
        }
    }
    
    public void PauseBackgroundMusic()
    {
        if (musicAudioSource != null && musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }
    
    public void ResumeBackgroundMusic()
    {
        if (musicAudioSource != null && !musicAudioSource.isPlaying)
        {
            musicAudioSource.UnPause();
        }
    }
    
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateVolumes();
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
    
    public float GetMasterVolume() => masterVolume;
    public float GetMusicVolume() => musicVolume;
    public float GetSFXVolume() => sfxVolume;
    
    // Metodă pentru redarea efectelor sonore cu volumul SFX
    public void PlaySoundEffect(AudioClip clip, Vector3 position)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, position, masterVolume * sfxVolume);
        }
    }
    
    public void PlaySoundEffect(AudioClip clip)
    {
        PlaySoundEffect(clip, Camera.main.transform.position);
    }
}
