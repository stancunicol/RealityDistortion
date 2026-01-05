using UnityEngine;

public class BadGuy : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip badGuyMusic;
    [SerializeField] [Range(0f, 1f)] private float volume = 0.8f;
    [SerializeField] private bool loop = true;
    
    private AudioSource audioSource;
    
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = badGuyMusic;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;
    }
    
    void OnEnable()
    {
        if (audioSource != null && badGuyMusic != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
    
    void OnDisable()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
    
    public void ResetAnomaly()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
