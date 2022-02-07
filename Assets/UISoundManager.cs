using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager Instance { get; private set; }
    [SerializeField]AudioClip selectaudio;
    [SerializeField]AudioClip clickaudio;
    [SerializeField] AudioSource _audioSource;

    void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
        _audioSource.playOnAwake = false;
    }
    public void PlaySelectedSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = selectaudio;
            _audioSource.Play();
        }
    }
    public void PlayClickSound()
    {
        if (!_audioSource.isPlaying)
        {
            _audioSource.clip = clickaudio;
            _audioSource.Play();
        }
    }
}